import { useEffect, useState } from "react";

const MOBILE_BREAKPOINT = 768;

interface UseDeviceTypeResult {
	isMobile: boolean;
	isDesktop: boolean;
}

export function useDeviceType(): UseDeviceTypeResult {
	const [isMobile, setIsMobile] = useState(
		typeof window !== "undefined"
			? window.innerWidth < MOBILE_BREAKPOINT
			: false,
	);

	useEffect(() => {
		const checkDevice = () => {
			setIsMobile(window.innerWidth < MOBILE_BREAKPOINT);
		};

		window.addEventListener("resize", checkDevice);
		return () => window.removeEventListener("resize", checkDevice);
	}, []);

	return {
		isMobile,
		isDesktop: !isMobile,
	};
}
