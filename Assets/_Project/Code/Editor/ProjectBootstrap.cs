using System.IO;
using BB.Core;
using BB.Data;
using BB.Presentation;
using BB.Simulation;
using BB.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace BB.Editor
{
    /// <summary>
    /// One-shot project bootstrap, runnable headless:
    ///   Unity -batchmode -quit -projectPath . -executeMethod BB.Editor.ProjectBootstrap.Run
    /// Creates layers, URP assets, the M1 greybox content (Chip + dummy +
    /// Stage_Greybox), placeholder flow scenes, and build settings.
    /// Idempotent: safe to re-run; existing assets are overwritten in place.
    /// </summary>
    public static class ProjectBootstrap
    {
        const string DataDir = "Assets/_Project/DataAssets";
        const string SettingsDir = "Assets/_Project/Settings";
        const string ScenesDir = "Assets/_Project/Scenes";
        const string PrefabsDir = "Assets/_Project/Prefabs/Fighters";

        public static void Run()
        {
            SetupLayers();
            SetupTime();
            SetupPlayerSettings();
            var material = SetupUrp();
            var controls = AssetDatabase.LoadAssetAtPath<InputActionAsset>($"{SettingsDir}/BackyardControls.inputactions");

            var chipJab = CreateAttack("Chip_Jab", AttackInput.Jab, startup: 4, active: 3, recovery: 8,
                new HitboxWindow { startTick = 4, endTick = 7, offset = new Vector3(0.55f, 0.45f, 0f), radius = 0.4f, damage = 6f, angleDegrees = 55f, baseKnockback = 25f, knockbackGrowth = 90f, hitstopTicks = 4 });
            var chipTilt = CreateAttack("Chip_ForwardTilt", AttackInput.ForwardTilt, startup: 8, active: 4, recovery: 14,
                new HitboxWindow { startTick = 8, endTick = 12, offset = new Vector3(0.8f, 0.55f, 0f), radius = 0.5f, length = 0.4f, damage = 11f, angleDegrees = 40f, baseKnockback = 40f, knockbackGrowth = 105f, hitstopTicks = 6 });
            var chipUpTilt = CreateAttack("Chip_UpTilt", AttackInput.UpTilt, startup: 6, active: 4, recovery: 12,
                new HitboxWindow { startTick = 6, endTick = 10, offset = new Vector3(0.1f, 1.0f, 0f), radius = 0.5f, damage = 9f, angleDegrees = 85f, baseKnockback = 35f, knockbackGrowth = 100f, hitstopTicks = 5 });

            var chipPrefab = CreateGreyboxFighterPrefab("Chip", material, new Color(0.55f, 0.53f, 0.5f), 0.35f, 0.8f);
            var chip = CreateFighter("Chip", "The Pebble", chipPrefab,
                weight: 130f, walk: 4f, run: 7.5f, air: 5.2f, jumpVel: 14f, gravity: 70f, maxFall: 16f, fastFall: 24f,
                radius: 0.35f, height: 0.8f, attacks: new[] { chipJab, chipTilt, chipUpTilt });

            var dummyPrefab = CreateGreyboxFighterPrefab("Dummy", material, new Color(0.8f, 0.4f, 0.35f), 0.35f, 1.0f);
            var dummy = CreateFighter("Dummy", "Training Dummy", dummyPrefab,
                weight: 100f, walk: 4.5f, run: 7.5f, air: 5.5f, jumpVel: 14f, gravity: 60f, maxFall: 14f, fastFall: 22f,
                radius: 0.35f, height: 1.0f, attacks: new[] { chipJab });

            var stage = CreateStageDefinition();
            var acorn = CreateAcornPickup();
            CreateRoster(chip, dummy);
            CreateRewardTable();

            CreateBootScene();
            CreatePlaceholderScene("MainMenu");
            CreatePlaceholderScene("Lobby");
            CreateGreyboxStageScene(stage, chip, dummy, controls, material, acorn);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene($"{ScenesDir}/Boot.unity", true),
                new EditorBuildSettingsScene($"{ScenesDir}/MainMenu.unity", true),
                new EditorBuildSettingsScene($"{ScenesDir}/Lobby.unity", true),
                new EditorBuildSettingsScene($"{ScenesDir}/Stage_Greybox.unity", true),
            };

            AssetDatabase.SaveAssets();
            Debug.Log("ProjectBootstrap: done.");
        }

        // ---- project settings ------------------------------------------------

        static void SetupLayers()
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layers = tagManager.FindProperty("layers");
            SetLayer(layers, 8, "Fighter");
            SetLayer(layers, 9, "Hurtbox");
            SetLayer(layers, 10, "Stage");
            tagManager.ApplyModifiedProperties();
        }

        static void SetLayer(SerializedProperty layers, int index, string name)
        {
            var element = layers.GetArrayElementAtIndex(index);
            if (string.IsNullOrEmpty(element.stringValue)) element.stringValue = name;
        }

        static void SetupTime()
        {
            var time = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TimeManager.asset")[0]);
            time.FindProperty("Fixed Timestep").floatValue = 1f / 60f;
            time.ApplyModifiedProperties();
        }

        static void SetupPlayerSettings()
        {
            PlayerSettings.productName = "Backyard Battle";
            PlayerSettings.companyName = "Max Finch Games";
        }

        static Material SetupUrp()
        {
            var rendererData = LoadOrCreate<UniversalRendererData>($"{SettingsDir}/URP_Renderer.asset");
            var pipelinePath = $"{SettingsDir}/URP_Asset.asset";
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(rendererData);
                AssetDatabase.CreateAsset(pipeline, pipelinePath);
            }
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;

            var matPath = $"{SettingsDir}/Greybox.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = new Color(0.7f, 0.7f, 0.7f);
                AssetDatabase.CreateAsset(mat, matPath);
            }
            return mat;
        }

        // ---- data assets -----------------------------------------------------

        static AttackDefinition CreateAttack(string name, AttackInput input, int startup, int active, int recovery, params HitboxWindow[] windows)
        {
            var attack = LoadOrCreate<AttackDefinition>($"{DataDir}/Moves/Attack_{name}.asset");
            attack.id = name.ToLowerInvariant();
            attack.displayName = name.Replace('_', ' ');
            attack.input = input;
            attack.startupTicks = startup;
            attack.activeTicks = active;
            attack.recoveryTicks = recovery;
            attack.hitboxes = windows;
            EditorUtility.SetDirty(attack);
            return attack;
        }

        static FighterDefinition CreateFighter(string name, string tagline, GameObject prefab,
            float weight, float walk, float run, float air, float jumpVel, float gravity, float maxFall, float fastFall,
            float radius, float height, AttackDefinition[] attacks)
        {
            var f = LoadOrCreate<FighterDefinition>($"{DataDir}/Fighters/Fighter_{name}.asset");
            f.id = name.ToLowerInvariant();
            f.displayName = name;
            f.tagline = tagline;
            f.fighterPrefab = prefab;
            f.weight = weight;
            f.walkSpeed = walk;
            f.runSpeed = run;
            f.airSpeed = air;
            f.jumpVelocity = jumpVel;
            f.airJumpVelocity = jumpVel * 0.9f;
            f.gravity = gravity;
            f.maxFallSpeed = maxFall;
            f.fastFallSpeed = fastFall;
            f.capsuleRadius = radius;
            f.capsuleHeight = height;
            f.attacks = attacks;
            // Brawler-feel accelerations: near-instant direction changes on the ground.
            f.groundAcceleration = run * 16f;
            f.groundFriction = 70f;
            f.airAcceleration = 48f;
            EditorUtility.SetDirty(f);
            return f;
        }

        static StageDefinition CreateStageDefinition()
        {
            var s = LoadOrCreate<StageDefinition>($"{DataDir}/Stages/Stage_Greybox.asset");
            s.id = "greybox";
            s.displayName = "Greybox Yard";
            s.tagline = "Where fighters are born. Never ships.";
            s.sceneName = "Stage_Greybox";
            s.blastZone = new Rect(-20f, -10f, 40f, 26f);
            s.cameraBounds = new Rect(-16f, -5f, 32f, 17f);
            s.spawnPoints = new[]
            {
                new Vector2(-3f, 1f), new Vector2(3f, 1f), new Vector2(-6f, 4.5f),
                new Vector2(6f, 4.5f), new Vector2(-1.5f, 1f), new Vector2(1.5f, 1f),
            };
            s.respawnPoint = new Vector2(0f, 6f);
            s.tournamentLegal = true;
            EditorUtility.SetDirty(s);
            return s;
        }

        static PickupDefinition CreateAcornPickup()
        {
            var p = LoadOrCreate<PickupDefinition>($"{DataDir}/Pickups/Pickup_Acorn.asset");
            p.id = "acorn";
            p.displayName = "Acorn";
            p.flavor = "Nature's fastball. The squirrels want it back.";
            p.throwSpeed = 15f;
            p.throwUpward = 3f;
            p.flightGravity = 25f;
            p.radius = 0.25f;
            p.damage = 9f;
            p.angleDegrees = 40f;
            p.baseKnockback = 55f;
            p.knockbackGrowth = 110f;
            p.hitstopTicks = 5;
            p.respawnSeconds = 8f;
            EditorUtility.SetDirty(p);
            return p;
        }

        static void CreateRoster(params FighterDefinition[] fighters)
        {
            var roster = LoadOrCreate<FighterRoster>($"{DataDir}/Roster.asset");
            roster.fighters = fighters;
            EditorUtility.SetDirty(roster);
        }

        static void CreateRewardTable()
        {
            var table = LoadOrCreate<RewardTable>($"{DataDir}/Awards/RewardTable.asset");
            EditorUtility.SetDirty(table);
        }

        // ---- prefab ----------------------------------------------------------

        static GameObject CreateGreyboxFighterPrefab(string name, Material baseMat, Color tint, float radius, float height)
        {
            string path = $"{PrefabsDir}/{name}_Greybox.prefab";

            var root = new GameObject(name);
            try
            {
                root.layer = LayerMask.NameToLayer("Fighter");

                var body = root.AddComponent<Rigidbody>();
                body.isKinematic = true;
                body.useGravity = false;

                var capsule = root.AddComponent<CapsuleCollider>();
                capsule.radius = radius;
                capsule.height = height;
                capsule.center = new Vector3(0f, height * 0.5f, 0f);

                var controller = root.AddComponent<FighterController>();
                controller.groundMask = LayerMask.GetMask("Stage");

                // Visual: tinted capsule + a nose cube so facing reads.
                var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                visual.name = "Body";
                Object.DestroyImmediate(visual.GetComponent<Collider>());
                visual.transform.SetParent(root.transform, false);
                visual.transform.localPosition = new Vector3(0f, height * 0.5f, 0f);
                visual.transform.localScale = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
                var visMat = new Material(baseMat) { color = tint };
                AssetDatabase.CreateAsset(visMat, $"{PrefabsDir}/{name}_Mat.mat");
                visual.GetComponent<MeshRenderer>().sharedMaterial = visMat;

                var nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
                nose.name = "Nose";
                Object.DestroyImmediate(nose.GetComponent<Collider>());
                nose.transform.SetParent(root.transform, false);
                nose.transform.localPosition = new Vector3(0f, height * 0.75f, radius * 0.9f);
                nose.transform.localScale = Vector3.one * radius * 0.5f;
                nose.GetComponent<MeshRenderer>().sharedMaterial = visMat;

                // Hurtbox: trigger capsule on its own layer, owner wired.
                var hurt = new GameObject("Hurtbox") { layer = LayerMask.NameToLayer("Hurtbox") };
                hurt.transform.SetParent(root.transform, false);
                var hurtCapsule = hurt.AddComponent<CapsuleCollider>();
                hurtCapsule.isTrigger = true;
                hurtCapsule.radius = radius;
                hurtCapsule.height = height;
                hurtCapsule.center = new Vector3(0f, height * 0.5f, 0f);
                hurt.AddComponent<Hurtbox>().owner = controller;

                return PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        // ---- scenes ----------------------------------------------------------

        static void CreateBootScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("GameManager").AddComponent<GameManager>();
            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/Boot.unity");
        }

        static void CreatePlaceholderScene(string name)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/{name}.unity");
        }

        static void CreateGreyboxStageScene(StageDefinition stage, FighterDefinition player, FighterDefinition dummy,
                                            InputActionAsset controls, Material material, PickupDefinition acorn)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Geometry — everything on the Stage layer.
            CreateBlock("Ground", new Vector3(0f, -0.5f, 0f), new Vector3(24f, 1f, 6f), material);
            CreateBlock("Platform_L", new Vector3(-6f, 3.5f, 0f), new Vector3(6f, 0.4f, 4f), material);
            CreateBlock("Platform_R", new Vector3(6f, 3.5f, 0f), new Vector3(6f, 0.4f, 4f), material);

            // Light.
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Camera.
            var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = camGo.AddComponent<Camera>();
            cam.fieldOfView = 45f;
            camGo.transform.position = new Vector3(0f, 4f, -16f);
            camGo.AddComponent<AudioListener>();
            camGo.AddComponent<UniversalAdditionalCameraData>();
            camGo.AddComponent<BrawlCamera>();

            // Match harness.
            var matchGo = new GameObject("Match");
            var match = matchGo.AddComponent<MatchController>();
            var starter = matchGo.AddComponent<DevMatchStarter>();
            starter.stage = stage;
            starter.player1Fighter = player;
            starter.dummyFighter = dummy;
            starter.dummyCount = 1;
            starter.controls = controls;
            matchGo.AddComponent<SimDebugOverlay>();
            matchGo.AddComponent<MatchJuice>();
            var hud = matchGo.AddComponent<MatchHud>();
            hud.match = match;

            // Contested center-stage acorn (the Power Stone moment).
            var pickupGo = new GameObject("Pickup_Acorn");
            pickupGo.transform.position = new Vector3(0f, 0.6f, 0f);
            var pickup = pickupGo.AddComponent<PickupItem>();
            pickup.definition = acorn;
            var acornVis = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            acornVis.name = "Visual";
            Object.DestroyImmediate(acornVis.GetComponent<Collider>());
            acornVis.transform.SetParent(pickupGo.transform, false);
            acornVis.transform.localScale = Vector3.one * 0.45f;
            var acornMat = new Material(material) { color = new Color(0.5f, 0.32f, 0.12f) };
            AssetDatabase.CreateAsset(acornMat, $"{SettingsDir}/Acorn_Mat.mat");
            acornVis.GetComponent<MeshRenderer>().sharedMaterial = acornMat;
            pickup.visual = acornVis;

            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/Stage_Greybox.unity");
        }

        static void CreateBlock(string name, Vector3 position, Vector3 scale, Material material)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.layer = LayerMask.NameToLayer("Stage");
            block.transform.position = position;
            block.transform.localScale = scale;
            block.GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        // ---- util ------------------------------------------------------------

        static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }
            return asset;
        }
    }
}
