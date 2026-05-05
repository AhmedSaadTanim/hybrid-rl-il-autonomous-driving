# 🧠 Curriculum-Based Hybrid Reinforcement and Imitation Learning for Autonomous Driving

## 📌 Overview

This project explores autonomous driving using a combination of Reinforcement Learning (RL) and Imitation Learning (IL) in a Unity ML-Agents environment.

A key contribution of this work is a **curriculum-based hybrid learning strategy**, which significantly improves performance and robustness.

---

## 🎯 Objectives

- Train an autonomous driving agent using RL (PPO)
- Evaluate Imitation Learning (Behavioral Cloning)
- Compare RL, IL, and Hybrid approaches
- Optimize RL–IL ratios
- Improve performance using curriculum learning

---

## 🏎️ Environment

- Engine: Unity ML-Agents  
- Track: Closed-loop racing environment  

### Observations
- Raycast sensors  
- Velocity (local X, Z)  
- Direction to next checkpoint  

### Actions
- Steering  
- Acceleration / Braking  

---

## 🧠 Methods

### Reinforcement Learning (RL)
- PPO algorithm  
- Reward-based learning  
- Unstable but powerful  

### Imitation Learning (IL)
- Behavioral Cloning  
- Stable but limited generalization  

### Hybrid Learning (RL + IL)
- Combines exploration + guidance  

---

## 🚀 ⭐ Main Contribution: Curriculum Learning

Training was done in **stages**:

1. **Perfect Lap Demo**
   - Agent learns optimal racing line  

2. **Recovery Demo**
   - Agent learns how to escape stuck states  

3. **Continued Training**
   - Combined behaviors into one policy  

👉 Final model: **E1**

---

## 📊 Results

### 🔥 Level 1 — RL vs IL vs Hybrid

![Cumulative Reward Comparison](results/figures/Cumulative%20Reward%20Comparison%20(RL%20vs%20IL%20vs%20Hybrid).png)

**Observation:**
- RL → unstable  
- IL → limited  
- Hybrid → best balance  

---

### ⚙️ Level 2 — Hybrid Optimization (99/1 vs 95/5)

![Hybrid Ratio](results/figures/Exploratory%20Hybrid%20Ratio%20Test%2099-1%20vs%2095-5.png)

**Observation:**
- **99% RL + 1% IL → Best**
- 95% RL + 5% IL → worse performance  

---

### 🏆 Best Model — E1 (Curriculum Learning)

#### Reward Progression
![E1 Reward](results/figures/E1_reward.png)

#### Episode Length
![E1 Episode Length](results/figures/E1_Episode%20Length.png)

**Performance:**
- Stable learning  
- Smooth driving  
- Strong recovery behavior  
- Highest reward (~150)  

---

## 🧪 Key Insights

- RL alone → unstable  
- IL alone → poor generalization  
- Hybrid → strong improvement  
- Curriculum → **major performance boost**  

---

## 🔍 Explainability

- Raycast-based perception visualization  
- Agent decisions influenced by:
  - Obstacle distance  
  - Checkpoint direction  
  - Velocity  

---

## 🎬 Demo

*(Add video here)*

---

## 🚀 Future Work

- Multi-track generalization  
- Better reward shaping  
- Vision-based learning  
- Advanced explainability  

---

## 🛠️ Tech Stack

- Unity ML-Agents  
- PPO  
- Behavioral Cloning  
- C#  

---

## 🧠 Key Takeaway

> Hybrid learning improves performance — but **curriculum learning makes it robust and practical**.

---

## 📬 Contact

Ahmed Saad Tanim  
Unity Developer | Software Engineer  
📧 ahmedsaadtanim@gmail.com  

---

## ⭐ Final Note

> The best model (E1) was achieved through **structured curriculum training**, not just hybrid learning.
