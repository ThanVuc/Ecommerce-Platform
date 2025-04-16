# Step 1: Build Angular app using Node.js
FROM node:18-alpine AS build

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install

COPY . .
RUN npm run build --prod

# Step 2: Serve the built app using Nginx
FROM nginx:alpine

# Copy built Angular app from the build stage
COPY --from=build /app/dist/eplatform-ui/browser /usr/share/nginx/html

# Expose the default Nginx port
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]