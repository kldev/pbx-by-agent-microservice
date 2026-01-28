import { useCallback } from "react";
import { useNavigate } from "react-router";

import { ROUTES } from "../routes";
import { useAuthStore } from "../stores/authStore";
import type { ApiErrorResponse } from "../api/rcp/models";

interface ApiError {
	status?: number;
	data?: ApiErrorResponse;
	message?: string;
}

export function useApiError() {
	const navigate = useNavigate();
	const clearUser = useAuthStore((state) => state.clearUser);

	const handleError = useCallback(
		(error: unknown) => {
			const apiError = error as ApiError;

			if (apiError?.status === 401) {
				clearUser();
				navigate(ROUTES.LOGIN);
				return;
			}

			if (apiError?.status === 403) {
				console.error("Access denied:", apiError.data?.message);
				return;
			}

			console.error("API Error:", apiError.data?.message || apiError.message);
		},
		[clearUser, navigate],
	);

	const getErrorMessage = useCallback((error: unknown): string => {
		const apiError = error as ApiError;

		if (apiError?.data?.message) {
			return apiError.data.message;
		}

		if (apiError?.message) {
			return apiError.message;
		}

		return "Wystąpił nieoczekiwany błąd";
	}, []);

	return { handleError, getErrorMessage };
}
