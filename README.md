# Deep-Learning-Cars
Cars trained with a genetic algorithm to navigate a track  
Try it out here https://rileykcostello.github.io/Deep-Learning-Cars/

**What do the different colors mean?**  
 * Blue cars are the cars from previous generations used to create new neural networks  
 * Pink cars are are a combination of two blue cars with the weights and biases of the network averaged  
 * Green cars are copies of blue cars with a few weights and biases randomized
 * Red cars are have completely randomized networks

**The Neural Network**  
Each car has a neural network with 5 inputs 2 hidden layers and 2 ouputs. Each input represents the length of a ray cast from the car. The hidden layers have 4 and 3 neurons respectively. 
The output controls the engine and turning of the car. The tanh function is used to keep activated neurons between -1 and 1.
