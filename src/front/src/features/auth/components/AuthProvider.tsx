import type React from "react";
import { useEffect, useRef } from "react";
import { useCurrentUser } from "../hooks";

interface AuthProviderProps {
	children: React.ReactNode;
}

const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
	const { checkAuth } = useCurrentUser();
	const hasChecked = useRef(false);

	useEffect(() => {
		if (!hasChecked.current) {
			hasChecked.current = true;
			checkAuth();
		}
	}, [checkAuth]);

	return <>{children}</>;
};

export default AuthProvider;
