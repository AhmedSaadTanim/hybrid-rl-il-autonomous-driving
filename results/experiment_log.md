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

RL overall:
- Cumulative reward improved significantly over training (~ -1000 to ~ -50 to -200 depending on seed)
- Learning trend visible but unstable across seeds
- Performance varied (seed 2 best, seed 1/3 weaker)
- Value loss decreased indicated policy learning effective
- Indicates RL can achieve strong performance but lacks stability and consistency

IL seed 01:
- Reward decreased significantly during training, reaching ~-800 before partial recovery
- Final performance remained poor (~-400 to -500)
- Pretraining loss decreased steadily, indicating learning from demonstrations
- However, learned behavior did not translate to stable performance
- Demonstrates lack of generalization in imitation learning

IL seed 02:
- Pretraining loss decreased steadily, indicating effective imitation learning
- Reward improved from ~-1000 to ~-200
- Performance better than seed 01 but still limited
- Demonstrates stable learning but weak performance compared to RL

IL seed 03:
- Pretraining loss decreased steadily (~0.75 to ~0.34)
- Indicates successful imitation learning from demonstrations
- Final cumulative reward ~ -330 (smoothed ~ -478)
- Performance unstable compared to seed 02
- Confirms IL produces consistent learning signal but inconsistent policy performance

IL overall:
- Pretraining loss consistently decreased across all seeds indicating stable learning
- Final rewards varied significantly across seeds (~ -200 to ~ -500)
- Indicates lack of generalization and error correction
- Demonstrates limitation of IL-only approach

HYB (99RL + 1IL) seed 01:
- Reward improved significantly (~ -400 to +50)
- Achieved best performance among all methods
- Training remained stable without collapse
- Pretraining loss decreased rapidly indicating early IL guidance
- Value loss decreased steadily indicating stable RL learning
- Demonstrates improved convergence and stability compared to RL-only

HYB (99RL + 1IL) seed 02:
- Reward improved significantly (~ -500 to +10)
- Training stable with smooth improvement curve
- No performance collapse observed
- Pretraining loss decreased rapidly indicating effective early guidance from demonstrations
- Value loss decreased steadily indicating stable learning dynamics
- Performance consistent with seed 01

HYB (99RL + 1IL) seed 03:
- Agent exhibited abnormal behavior (reverse driving) during training
- Reward remained low (~ -300 to -450) with no stable improvement
- Training unstable compared to seed 01 and seed 02
- Indicates hybrid method still has some seed sensitivity

HYB (99RL + 1IL) seed 04:
- Reward remained negative (~ -250 to -300) with limited improvement
- Training unstable with no consistent convergence
- Pretraining loss decreased indicating imitation learning occurred
- However, learned behavior did not translate to strong performance
- Similar failure pattern observed as seed 03

HYB (99RL + 1IL) overall:
- Achieved highest performance among all methods (positive rewards in some seeds)
- Demonstrates clear improvement over RL-only and IL-only approaches
- However, performance remains seed-dependent with some unstable runs (seed 03, 04)
- Pretraining loss consistently decreased indicating effective use of demonstrations
- Shows that small IL contribution improves performance but does not fully eliminate instability

Hybrid (95% RL, 5% IL):
- Initial performance improvement observed during early training
- However, training became unstable and reward degraded significantly
- Final performance (~ -300) worse than 99/1 configuration
- Indicates excessive reliance on imitation reduces policy adaptability

Comparison:
- 99/1 achieved best performance and stability
- 95/5 suffered from overfitting to demonstrations
- Confirms small IL guidance is beneficial, but higher IL influence is detrimental