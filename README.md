# Deploy a 3-Tier Application on Amazon EKS Cluster
Deploy a 3-tier .NET Core application (frontend UI, .NET API, and PostgreSQL database) on an Amazon EKS cluster using Kubernetes manifests.

## Overview
The [3-tier application]( https://github.com/InstructorDiogo/3-tier-application/tree/main) consists of:
- A backend (.NET 7) called Basic3TierAPI
- A frontend (HTML/CSS/JavaScript) served via Nginx, called Basic3TierUI
- A PostgreSQL database

## Steps
1. Containerize each service (frontend and backend) and store them in Docker Hub. The PostgreSQL database will run directly from the official postgres image in your cluster.
2. Provision an EKS cluster in Amazon AWS (you can use ```eksctl``` or the AWS console) and ```kubectl``` configured to connect to your EKS cluster.
3. Create a Kubernetes deployment for frontend as well as service manifest to expose the deployment. 
4. Create a deployment + service manifest for backend.
5. Create deployment + service manifest for database, along with persistent storage for PostgreSQL. (Pre-requisite: StorageClass configured to create persistent volumes (e.g., gp2 or gp3 on AWS))
6. Install NGINX Ingress Controller via Helm
7. Create an Ingress resource to expose the frontend application externally.

### 1. Containerize the UI and API
Build and Push the API Image:
```bash
cd 3-tier-application/api
docker build -t <your-dockerhub-username>/basic3tier-api:latest .
docker login
docker push <your-dockerhub-username>/basic3tier-api:latest
```
Build and Push the UI Image:
```bash
cd 3-tier-application/ui
docker build -t <your-dockerhub-username>/basic3tier-ui:latest .
docker push <your-dockerhub-username>/basic3tier-ui:latest
```

### 2. EKS Cluster Setup
```
eksctl create cluster \
  --name <your-cluster-name> \
  --region <aws-region> \
  --nodegroup-name standard-workers \
  --node-type t3.medium \
  --nodes <2-or-3>
```
```
eksctl get cluster
kubectl get nodes
kubectl config get-contexts
```
### 3. Create the API Deployment & Service
After creating the manifests, deploy:
```
kubectl apply -f api-deployment.yaml
```
### 4. Create UI Deployment & Service
After creating the manifests, deploy:
```
kubectl apply -f ui-deployment.yaml
```
### 5. Create Deployment and Service manifests for the PostgreSQL
After creating the manifests, deploy:
```
kubectl apply -f <deployment-name.yaml>
```
Create persistent volume and PVC for PostgreSQL, then 
```
kubectl apply -f postgres-pv.yaml
```

Verify everything is running:
```
kubectl get deployments
kubectl get pods
kubectl get svc
kubectl get pv,pvc
```
### 6. Install NGINX Ingress Controller
Run the following command to deploy the Nginx Ingress Controller using Helm in the Correct Namespace:
```
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update
helm search repo ingress-nginx
```
```
kubectl create namespace ingress-nginx
helm install nginx-ingress ingress-nginx/ingress-nginx \
  --namespace ingress-nginx \
  --set controller.replicaCount=2 \
  --set controller.service.type=LoadBalancer
```
Verify the installation:
```
kubectl get pods --namespace ingress-nginx -w
kubectl get svc --namespace ingress-nginx
```

### 7. Exposing the Frontend with an Ingress
To make the UI accessible from the internet, an Ingress Controller (e.g., ALB Ingress Controller or NGINX Ingress Controller) is needed.




