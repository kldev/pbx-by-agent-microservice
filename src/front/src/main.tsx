import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router";
import "./index.css";
import { FluentProvider, QueryProvider } from "./providers";
import { router } from "./routes";

// Set document title from env
document.title = import.meta.env.VITE_APP_NAME || "PBX by Agent";

const rootElement = document.getElementById("root");
if (!rootElement) throw new Error("Root element not found");

createRoot(rootElement).render(
	<StrictMode>
		<QueryProvider>
			<FluentProvider>
				<RouterProvider router={router} />
			</FluentProvider>
		</QueryProvider>
	</StrictMode>,
);
