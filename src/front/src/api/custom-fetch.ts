const API_BASE_URL = import.meta.env.VITE_API_URL ?? "";
const TOKEN_KEY = "auth_token";

const getAuthHeaders = (): Record<string, string> => {
	const token = localStorage.getItem(TOKEN_KEY);
	if (token) {
		return { Authorization: `Bearer ${token}` };
	}
	return {};
};

export const customFetch = async <T>(
	url: string,
	options?: RequestInit,
): Promise<T> => {
	const response = await fetch(`${API_BASE_URL}${url}`, {
		...options,
		credentials: "include",
		headers: {
			"Content-Type": "application/json",
			...getAuthHeaders(),
			...options?.headers,
		},
	});

	const status = response.status;
	const headers = response.headers;

	if (status === 204) {
		return { data: undefined, status, headers } as T;
	}

	const data = await response.json().catch(() => ({}));

	return { data, status, headers } as T;
};
