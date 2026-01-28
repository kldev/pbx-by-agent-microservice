import { useCallback, useEffect } from "react";
import { useGetMe } from "../../../api/identity/endpoints/auth/auth";
import { useAuthStore } from "../../../stores";

export function useCurrentUser() {
	const {
		user,
		isAuthenticated,
		isLoading,
		setUser,
		clearUser,
		setLoading,
		getToken,
	} = useAuthStore();

	const hasToken = !!getToken();

	const getMeMutation = useGetMe();

	// Handle mutation result
	useEffect(() => {
		if (getMeMutation.isSuccess && getMeMutation.data) {
			if (getMeMutation.data.status === 200) {
				setUser(getMeMutation.data.data);
			} else {
				clearUser();
			}
			setLoading(false);
		}
		if (getMeMutation.isError) {
			clearUser();
			setLoading(false);
		}
	}, [
		getMeMutation.isSuccess,
		getMeMutation.isError,
		getMeMutation.data,
		setUser,
		clearUser,
		setLoading,
	]);

	// Auto-fetch on mount if has token
	useEffect(() => {
		if (hasToken && !user && !getMeMutation.isPending) {
			getMeMutation.mutate();
		}
	}, [hasToken, user, getMeMutation]);

	const checkAuth = useCallback(() => {
		if (!hasToken) {
			clearUser();
			return;
		}
		setLoading(true);
		getMeMutation.mutate();
	}, [hasToken, clearUser, setLoading, getMeMutation]);

	return {
		user,
		isAuthenticated,
		isLoading: isLoading || getMeMutation.isPending,
		checkAuth,
		error: getMeMutation.error,
	};
}
