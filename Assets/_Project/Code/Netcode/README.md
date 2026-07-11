# BB.Netcode (arrives at M4)

Fish-Net v4 wrappers live here from milestone M4 onward: `NetworkFighter`
(`[Replicate]`/`[Reconcile]` prediction over the `BB.Simulation` motor), session
and lobby flow, transport config (Tugboat UDP).

Intentionally empty until then — no asmdef so the empty assembly doesn't
generate compile warnings. The simulation layer is already network-shaped:
tick-driven, consumes `InputSnapshot` structs, keeps all fighter state in a
plain serializable struct, and resolves hits server-side through one code path.
