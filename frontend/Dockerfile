# Stage 1: Build the React app
FROM node:19-alpine AS build

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json files to the container
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the app's source code to the container
COPY . .

# Build the app
RUN npm run build

# Stage 2: Serve the app using Nginx
FROM nginx:1.21.0-alpine

# Copy custom nginx configuration file
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy build files from the 'build' stage
COPY --from=build /app/build /usr/share/nginx/html

# Expose port 80 to the outside world
EXPOSE 80

# Start Nginx when the container starts
CMD ["nginx", "-g", "daemon off;"]
