# COMP303-2203760

![image](https://github.falmouth.ac.uk/GA-Undergrad-Student-Work-24-25/COMP303-2203760/blob/main/Images/artefact.png)

# Navigation Mesh Generation
This artefact used in an experiment to test the performance of a bespoke Navigation Mesh solution against Unity's NavMesh generation solution. These two solution are implimented in two seperate Unity scenes, named appropriately. The custom solution uses the iterative mesh triangulation algorithm known as the Bowyer-Watson algorithm that makes use of the Delaunay triangulation algorithm. The other solution is Unity's built in triangulation method for the NavMesh. Both solutions are tested on the same meshes. These meshes are of two types, the semi-random grid-like meshes and the completely random meshes.

# Build Instructions
This project has been constructed to run in the Unity Editor.

1. Clone the repository.
2. Open one of two scenes, "NavMesh" for the custom solution and "UnityNavMesh" for Unity's NavMesh solution.
3. Press play in the editor, you can press the key 'Enter' to run the algorithm again.
