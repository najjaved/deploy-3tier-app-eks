# Use an official Nginx runtime as the base image
FROM nginx:alpine

# Copy your frontend files (HTML, JavaScript, CSS, etc.) to the default Nginx web root directory
COPY . /usr/share/nginx/html

# Expose port 80, which Nginx will listen on
EXPOSE 80

# Start Nginx when the container is run
CMD ["nginx", "-g", "daemon off;"]
