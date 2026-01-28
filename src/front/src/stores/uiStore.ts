import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { ThemeMode } from "../theme";

interface UIState {
	isSidebarCollapsed: boolean;
	theme: ThemeMode;
	toggleSidebar: () => void;
	setSidebarCollapsed: (collapsed: boolean) => void;
	setTheme: (theme: ThemeMode) => void;
	toggleTheme: () => void;
}

export const useUIStore = create<UIState>()(
	persist(
		(set) => ({
			isSidebarCollapsed: false,
			theme: "light",
			toggleSidebar: () =>
				set((state) => ({ isSidebarCollapsed: !state.isSidebarCollapsed })),
			setSidebarCollapsed: (collapsed) =>
				set({ isSidebarCollapsed: collapsed }),
			setTheme: (theme) => set({ theme }),
			toggleTheme: () =>
				set((state) => ({ theme: state.theme === "light" ? "dark" : "light" })),
		}),
		{
			name: "ui-storage",
		},
	),
);
