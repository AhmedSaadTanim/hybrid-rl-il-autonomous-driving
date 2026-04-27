RL seed 01:
- Reward improved significantly from ~-1000 to ~-20
- Training showed instability with performance dropping after peak
- Agent behavior improved but was inconsistent
- Indicates RL alone struggles with stable convergence

RL seed 02:
- Reward improved steadily from ~-700 to ~-50
- Training was significantly more stable than seed 01
- No major performance collapse observed
- Indicates RL performance is highly dependent on initialization (seed variance)

RL seed 03:
- Reward showed partial improvement but degraded toward the end
- Less stable compared to seed 02
- Demonstrates inconsistency in learning behavior across seeds

IL seed 01:
- Reward decreased significantly during training, reaching ~-800 before partial recovery
- Final performance remained poor (~-400 to -500)
- Pretraining loss decreased steadily, indicating learning from demonstrations
- However, learned behavior did not translate to stable performance
- Demonstrates lack of generalization in imitation learning