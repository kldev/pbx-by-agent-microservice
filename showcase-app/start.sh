#!/bin/bash

# JD Showcase App - Start Script
# ==============================

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "=========================================="
echo "  PBX Microservices Showcase"
echo "=========================================="
echo ""

# Parse arguments
BUILD_ARG=""
DETACH_ARG="-d"

while [[ $# -gt 0 ]]; do
    case $1 in
        --build)
            BUILD_ARG="--build"
            shift
            ;;
        --foreground|-f)
            DETACH_ARG=""
            shift
            ;;
        --clean)
            echo "Cleaning up volumes and containers..."
            docker-compose down -v --remove-orphans
            echo "Clean complete."
            exit 0
            ;;
        --help|-h)
            echo "Usage: ./start.sh [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --build       Force rebuild of all images"
            echo "  --foreground  Run in foreground (see logs)"
            echo "  --clean       Remove all containers and volumes"
            echo "  --help        Show this help"
            echo ""
            echo "After starting, access:"
            echo "  Gateway API:    http://localhost:8080"
            echo ""
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

echo "Starting services..."
echo ""

docker-compose up $BUILD_ARG $DETACH_ARG

if [ -n "$DETACH_ARG" ]; then
    echo ""
    echo "=========================================="
    echo "  Services started successfully!"
    echo "=========================================="
    echo ""
    echo "Endpoints:"
    echo "  Gateway API:    http://localhost:8080"
    echo "  Health checks:"
    echo "    - Gateway:    http://localhost:8080/health"
    echo ""
    echo ""
    echo "Swagger UI (via Gateway):"
    echo "  - Identity:     (internal only)"
    echo "  - Rcp:         (internal only)"
    echo "  - Rate:        (internal only)"
    echo ""
    echo "Commands:"
    echo "  View logs:      docker-compose logs -f"
    echo "  Stop services:  docker-compose down"
    echo "  Clean all:      ./start.sh --clean"
    echo ""
fi
