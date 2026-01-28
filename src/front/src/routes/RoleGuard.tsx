import type React from "react";
import { Outlet } from "react-router";
import { AccessDeniedPage } from "../features/auth/pages";
import { useAuthStore } from "../stores/authStore";

interface RoleGuardProps {
	allowedRoles: string[];
	children?: React.ReactNode;
}

/**
 * Komponent do ochrony routów na podstawie ról użytkownika.
 * Jeśli użytkownik nie ma wymaganej roli, wyświetla AccessDeniedPage.
 */
const RoleGuard: React.FC<RoleGuardProps> = ({ allowedRoles, children }) => {
	const user = useAuthStore((state) => state.user);
	const userRoles = user?.roles ?? [];

	const hasAccess = userRoles.some((role) => allowedRoles.includes(role));

	if (!hasAccess) {
		return <AccessDeniedPage requiredRoles={allowedRoles} />;
	}

	return children ? <>{children}</> : <Outlet />;
};

export default RoleGuard;
