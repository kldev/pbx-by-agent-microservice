# Stage 1: Build
FROM node:22-alpine AS builder

# Build arguments for environment variables
ARG VITE_API_URL=""
ARG VITE_APP_ENV=production
ARG VITE_APP_MODE="full"
ARG VITE_DEFAULT_EMAIL=""
ARG VITE_DEFAULT_PASSWORD=""

# Set working directory
WORKDIR /app

# Enable Corepack for Yarn 4
RUN corepack enable

# Copy package files
COPY package.json yarn.lock ./

# Install dependencies
RUN yarn install --immutable

# Copy source files
COPY . .

# Set environment variables for build
ENV VITE_API_URL=${VITE_API_URL}
ENV VITE_APP_ENV=${VITE_APP_ENV}
ENV VITE_APP_MODE=${VITE_APP_MODE}
ENV VITE_DEFAULT_EMAIL=${VITE_DEFAULT_EMAIL}
ENV VITE_DEFAULT_PASSWORD=${VITE_DEFAULT_PASSWORD}

# Build the application
RUN yarn build

# Stage 2: Production
FROM nginx:alpine

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built files from builder stage
COPY --from=builder /app/dist /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
