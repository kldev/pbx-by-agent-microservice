import { useQueryClient } from "@tanstack/react-query";
import { useCallback, useState } from "react";
import { useNavigate } from "react-router";
import { ROUTES } from "../../../routes";
import { useAuthStore } from "../../../stores";
import { useCurrentUser } from "./useCurrentUser";

export function useAuth() {
	const navigate = useNavigate();
	const queryClient = useQueryClient();
	const { clearUser } = useAuthStore();
	const { user, isAuthenticated, isLoading, checkAuth, error } =
		useCurrentUser();
	const [isLoggingOut, setIsLoggingOut] = useState(false);

	const logout = useCallback(() => {
		setIsLoggingOut(true);
		// Clear local auth state (JWT token is stored in localStorage)
		clearUser();
		// Clear react-query cache to prevent stale data after re-login
		queryClient.clear();
		setIsLoggingOut(false);
		navigate(ROUTES.LOGIN);
	}, [clearUser, queryClient, navigate]);

	return {
		user,
		isAuthenticated,
		isLoading,
		checkAuth,
		logout,
		isLoggingOut,
		error,
	};
}
