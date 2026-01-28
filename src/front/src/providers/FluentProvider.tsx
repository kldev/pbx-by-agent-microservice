import { FluentProvider as FluentUIProvider } from "@fluentui/react-components";
import type React from "react";
import { useUIStore } from "../stores";
import { getTheme } from "../theme";

interface FluentProviderProps {
	children: React.ReactNode;
}

const FluentProvider: React.FC<FluentProviderProps> = ({ children }) => {
	const theme = useUIStore((state) => state.theme);

	return (
		<FluentUIProvider theme={getTheme(theme)}>{children}</FluentUIProvider>
	);
};

export default FluentProvider;
