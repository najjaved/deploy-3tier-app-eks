# Deploy a 3-Tier Application on Amazon EKS Cluster
Deploy a 3-tier .NET Core application (frontend UI, .NET API, and PostgreSQL database) on an Amazon EKS cluster using Kubernetes manifests.

## Overview
The [3-tier application]( https://github.com/InstructorDiogo/3-tier-application/tree/main) consists of:
- A backend (.NET 7) called Basic3TierAPI
- A frontend (HTML/CSS/JavaScript) served via Nginx, called Basic3TierUI
- A PostgreSQL database

## Steps
1. Provision an EKS cluster in Amazon AWS (you can use ```eksctl``` or the AWS console) and ```kubectl``` configured to connect to your EKS cluster.
2. Containerize each service (frontend and backend) and store them in Docker Hub. The PostgreSQL database will run directly from the official postgres image in your cluster.
3. Create a Kubernetes deployment for frontend as well as service manifest to expose the deployment. 
4. Create a deployment + service manifest for backend.
5. Create deployment + service manifest for database, along with persistent storage for PostgreSQL. (Pre-requisite: StorageClass configured to create persistent volumes (e.g., gp2 or gp3 on AWS))
6. Install Ingress controller via helm
7. Create an Ingress resource to expose the frontend application externally.

