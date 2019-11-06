# SmartRocket with GA
smart rocket with Genetic Algorithm simulation

## Movie
[Youtube](https://youtu.be/mZ0FwMoI0Ew)

## Description
This simulation tries to solve the better path to reach the target with gentic algorithm in 2D space

## こだわりポイント (The thing I put an effort)
1. プロセスの可視化 / visualization of the process <br>
個体の適応度を直感的にわかりやすくするために、1世代前の適応度に基づいて軌跡の濃度を変えた。適応度がよければ良いほど（即ちターゲットへの距離が近ければ近いほど）、軌跡が美しく描かれる。 <br>
To visualize the fitness of each individual, I changed the intensity of the color based on the fitness of previous generation. The better the fitness is, the more beautiful the trail is. 

2. インタラクション / interaction <br>
自由にターゲットの位置を変えれるようにしたことでユーザーもシミュレーションに参加できる。 <br>
The user can participate in the simulation by changing the position of the target.

## GA
### Crossover
- One point crossover
Chromsome is an array of vectors. Since the sequence of those vectors is important for this case, I use one point crossover which normally does not destroy the sequence that much. 
### Selection
- Roulette selection
This selection is simply following the thoght of "The closer the distance to the target is, the higher the fitness is".
This is a simple algorithm, but powerful to inherit the better genom.

## Discovery
Gentic Algorithm is not working in the way which I expected if I set many constraint conditions.
This is a pretty intersting result for me because I thought that this result reflects the importance of diversity in the nature. I needed to keep a room for flexibility which helps individuals to handle with unexpected environments.
