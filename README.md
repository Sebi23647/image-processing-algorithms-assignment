Image Processing Project

Overview

This repository contains a desktop image processing application developed in C# (.NET Framework 4.8). The project was created in an academic context and includes a collection of classical image processing algorithms,
implemented as individual assignments.
The base architecture and graphical user interface were provided as educational support, while my contribution focuses on the implementation of the algorithms listed below.

The project demonstrates my ability to work on an existing codebase, understand a predefined architecture, and integrate new functionality without disrupting existing components.

Disclaimer

This project is not entirely my own work.
The application structure, GUI, and part of the codebase were provided by the course instructor.
My contributions are located primarily in the Algorithms folder and the corresponding logic in MenuCommands.cs.

This repository is published for educational and demonstration purposes only.

Implemented Algorithms

Tools

Binary – Converts the image to black and white based on a threshold.

Crop – Crops a selected region of the image.

Mirror – Horizontally mirrors the image.

Rotate clockwise – Rotates the image 90° clockwise.

Rotate anti-clockwise – Rotates the image 90° counterclockwise.

Pointwise Operations

Spline Tool – Applies a user-defined pointwise transformation using spline interpolation.

Thresholding

Binary Triangle – Automatically determines the optimal binarization threshold using the Triangle method.

Filters

Median Filter – Reduces noise using median filtering.

LoG Filter (Laplacian of Gaussian) – Enhances edges in the image.

Morphological Operations

Connected Components – Detects and labels connected components in a binary image.

Geometrical Transformations

Spherical Deformation – Applies a spherical deformation effect to the image.

Segmentation

K-Means Clustering – Segments the image into k regions using the k-means algorithm.

Project Structure

Algorithms/ – algorithm implementations developed by me
MenuCommands.cs – integration of algorithms into the application menu
remaining components – infrastructure and UI provided as educational material

Technologies Used

C#
.NET Framework 4.8
Windows Forms
Visual Studio

License & Usage

This project is intended for educational purposes only and serves as an academic assignment. It is not designed or maintained for production use.
