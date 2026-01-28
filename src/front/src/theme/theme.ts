import {
	type BrandVariants,
	createDarkTheme,
	createLightTheme,
	type Theme,
} from "@fluentui/react-components";

// Custom brand colors for the application
const brandColors: BrandVariants = {
	10: "#020305",
	20: "#111723",
	30: "#16263D",
	40: "#193253",
	50: "#1B3F6A",
	60: "#1B4C82",
	70: "#18599B",
	80: "#1267B4",
	90: "#3174C2",
	100: "#4F82C8",
	110: "#6790CF",
	120: "#7D9ED5",
	130: "#92ACDC",
	140: "#A6BAE2",
	150: "#BAC9E9",
	160: "#CDD8EF",
};

export const lightTheme: Theme = {
	...createLightTheme(brandColors),
};

export const darkTheme: Theme = {
	...createDarkTheme(brandColors),
};

// Ensure the dark theme has proper background
darkTheme.colorBrandForeground1 = brandColors[110];
darkTheme.colorBrandForeground2 = brandColors[120];

export type ThemeMode = "light" | "dark";

export const getTheme = (mode: ThemeMode): Theme => {
	return mode === "light" ? lightTheme : darkTheme;
};
