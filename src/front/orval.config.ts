import { defineConfig } from "orval";

const GATEWAY_URL = "http://localhost:8080";

const commonOutput = {
	mode: "tags-split" as const,
	client: "react-query" as const,
	httpClient: "fetch" as const,
	clean: true,
	prettier: false,
	override: {
		mutator: {
			path: "./src/api/custom-fetch.ts",
			name: "customFetch",
		},
		query: {
			useQuery: true,
			useMutation: true,
		},
	},
};

export default defineConfig({
	gateway: {
		input: {
			target: `${GATEWAY_URL}/swagger/v1/swagger.json`,
		},
		output: {
			...commonOutput,
			target: "./src/api/gateway/endpoints",
			schemas: "./src/api/gateway/models",
		},
	},
	identity: {
		input: {
			target: `${GATEWAY_URL}/api-docs/identity/swagger.json`,
		},
		output: {
			...commonOutput,
			target: "./src/api/identity/endpoints",
			schemas: "./src/api/identity/models",
		},
	},
	dataSource: {
		input: {
			target: `${GATEWAY_URL}/api-docs/datasource/swagger.json`,
		},
		output: {
			...commonOutput,
			target: "./src/api/datasource/endpoints",
			schemas: "./src/api/datasource/models",
		},
	},
	rcp: {
		input: {
			target: `${GATEWAY_URL}/api-docs/rcp/swagger.json`,
		},
		output: {
			...commonOutput,
			target: "./src/api/rcp/endpoints",
			schemas: "./src/api/rcp/models",
		},
	},
	rate: {
		input: {
			target: `${GATEWAY_URL}/api-docs/rate/swagger.json`,
		},
		output: {
			...commonOutput,
			target: "./src/api/rate/endpoints",
			schemas: "./src/api/rate/models",
		},
	},
	cdr: {
		input: {
			target: `${GATEWAY_URL}/api-docs/cdr/swagger.json`,
		},
		output: {
			...commonOutput,
			target: "./src/api/cdr/endpoints",
			schemas: "./src/api/cdr/models",
		},
	},
	answerrule: {
		input: {
			target: `${GATEWAY_URL}/api-docs/answerrule/swagger.json`,
		},
		output: {
			...commonOutput,
			target: "./src/api/answerrule/endpoints",
			schemas: "./src/api/answerrule/models",
		},
	},
});
