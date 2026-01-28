import react from "@vitejs/plugin-react";
import { visualizer } from "rollup-plugin-visualizer";
import { defineConfig } from "vite";

// https://vite.dev/config/
export default defineConfig({
	plugins: [
		react(),
		visualizer({
			filename: "./dist/stats.html",
			open: false,
			gzipSize: true,
			brotliSize: true,
		}),
	],
	server: {
		port: 4300,
		strictPort: true,
	},
	build: {
		sourcemap: false,
		rollupOptions: {
			output: {
				manualChunks: {
					// Vendor chunks - duże biblioteki osobno
					"vendor-react": ["react", "react-dom", "react-router", "scheduler"],
					"vendor-fluent": [
						"@fluentui/react-components",
						"@fluentui/react-icons",
						"@fluentui/react-datepicker-compat",
					],
					"vendor-forms": ["react-hook-form", "@hookform/resolvers", "zod"],
					"vendor-query": ["@tanstack/react-query"],
					"vendor-dnd": [
						"@dnd-kit/core",
						"@dnd-kit/sortable",
						"@dnd-kit/utilities",
					],
					"vendor-state": ["zustand"],
				},
			},
		},
		chunkSizeWarningLimit: 600,
	},
});
