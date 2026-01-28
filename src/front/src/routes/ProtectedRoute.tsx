import { makeStyles, Spinner } from "@fluentui/react-components";
import type React from "react";
import { Suspense, useEffect } from "react";
import { Navigate, useLocation } from "react-router";
import { AppShell } from "../components/layout";
import { useCurrentUser } from "../features/auth/hooks";
import { useAuthStore } from "../stores/authStore";
import { ROUTES } from "./routes";

const useStyles = makeStyles({
	loadingContainer: {
		display: "flex",
		alignItems: "center",
		justifyContent: "center",
		height: "100vh",
		width: "100vw",
	},
});

const ProtectedRoute: React.FC = () => {
	const styles = useStyles();
	const location = useLocation();
	const { isAuthenticated, isLoading, hasCheckedAuth } = useAuthStore();
	const { checkAuth } = useCurrentUser();

	useEffect(() => {
		if (!hasCheckedAuth && !isLoading) {
			checkAuth();
		}
	}, [hasCheckedAuth, isLoading, checkAuth]);

	// Pokaż spinner tylko gdy sprawdzamy auth
	if (!hasCheckedAuth || isLoading) {
		return (
			<div className={styles.loadingContainer}>
				<Spinner size="large" label="Sprawdzanie sesji..." />
			</div>
		);
	}

	// Po sprawdzeniu - jeśli nie zalogowany, przekieruj
	if (!isAuthenticated) {
		return <Navigate to={ROUTES.LOGIN} state={{ from: location }} replace />;
	}

	return (
		<Suspense
			fallback={
				<div className={styles.loadingContainer}>
					<Spinner size="large" label="Ładowanie..." />
				</div>
			}
		>
			<AppShell />
		</Suspense>
	);
};

export default ProtectedRoute;
