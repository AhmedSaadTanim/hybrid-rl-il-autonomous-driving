🧠 Hybrid Reinforcement and Imitation Learning for Autonomous Driving in Simulation

📌 Overview

This project explores the effectiveness of hybrid learning strategies combining Reinforcement Learning (RL) and Imitation Learning (IL) for training autonomous driving agents in a simulated environment built with Unity.

While pure RL struggled to achieve stable performance, hybrid approaches significantly improved convergence speed and driving behavior. This project investigates different RL–IL ratios and analyzes their impact on agent performance.

🎯 Objectives

Train an autonomous driving agent using RL (PPO).
Compare performance with Imitation Learning (Behavior Cloning).
Evaluate hybrid RL + IL strategies.
Analyze training stability and performance.
Provide interpretable insights into agent behavior.

🏎️ Environment

Engine: Unity (ML-Agents)

Track: Closed-loop racing environment with checkpoints

Observations: Raycast sensors (obstacle detection), 
Velocity (local x, z), 
Direction to next checkpoint

Actions: Steering, Acceleration / Braking

🧠 Methods

1. Reinforcement Learning (RL)
Algorithm: PPO (Proximal Policy Optimization)
Reward signals:
Positive reward for correct checkpoint
Penalty for wrong direction
Collision penalties
2. Imitation Learning (IL)
Method: Behavior Cloning
Training from demonstration data
3. Hybrid Learning (RL + IL)

Different ratios of RL and IL were tested:

RL (%) -	IL (%)
[70:30, 
50:50, 
30:70,
95:5]

Based on the results, the final validation focuses on RL-only, IL-only, and the best-performing hybrid configuration 95:5

📊 Experiments

Training Setup

Total steps: 500,000 per run, 
Multiple runs per configuration

Evaluation metrics: Mean reward, Collision frequency, Checkpoint completion

Key Observations

1. RL-only training showed slow convergence and instability

2. IL-only training lacked generalization

3. Hybrid approaches: Improved learning speed, Reduced collision rates, Produced smoother navigation behavior

👉 Best performance achieved with high RL + small IL (e.g., 95:5)

🔍 Explainability

To better understand agent decisions:

Visual Explanation
Raycast sensors visualized in Unity
Colors indicate obstacle proximity
Decision Analysis (Example)
Action: Turn Left
Top Influences:
- Right ray distance (low → obstacle avoidance)
- Direction to checkpoint (left)
- Current velocity

This helps interpret how the agent reacts to its environment.

📈 Results

Metrics Compared
Mean reward over time
Stability of learning
Driving smoothness

(Insert graphs here)

🎬 Demo

(Insert video link here)

🚀 Future Work

* Multi-agent environments
  
* Generalization across different tracks
  
* Integration with VR/XR for immersive analysis
  
* Advanced explainability methods (e.g., LIME-style perturbations)
  
🧠 Key Takeaway

Hybrid learning strategies can significantly improve RL performance in complex environments by combining structured guidance (IL) with exploration (RL).

📬 Contact

Ahmed Saad Tanim
(Unity Developer | Software Engineer | ahmedsaadtanim@gmail.com)
