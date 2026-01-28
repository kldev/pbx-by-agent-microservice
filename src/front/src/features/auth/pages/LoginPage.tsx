import {
	Button,
	Card,
	Field,
	Input,
	makeStyles,
	Spinner,
	Text,
	Title1,
	tokens,
} from "@fluentui/react-components";
import { LockClosedRegular, PersonRegular } from "@fluentui/react-icons";
import type React from "react";
import { useEffect, useRef, useState } from "react";
import { Link, Navigate, useNavigate } from "react-router";
import {
	type loginResponse,
	useLogin,
} from "../../../api/gateway/endpoints/auth/auth";
import { ROUTES } from "../../../routes";
import { useAuthStore } from "../../../stores/authStore";
import { useCurrentUser } from "../hooks";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		alignItems: "center",
		justifyContent: "center",
		minHeight: "100vh",
		backgroundColor: tokens.colorNeutralBackground2,
		padding: "24px",
	},
	card: {
		width: "100%",
		maxWidth: "400px",
		padding: "32px",
	},
	form: {
		display: "flex",
		flexDirection: "column",
		gap: "20px",
	},
	header: {
		textAlign: "center",
		marginBottom: "24px",
	},
	subtitle: {
		color: tokens.colorNeutralForeground3,
		marginTop: "8px",
	},
	error: {
		color: tokens.colorPaletteRedForeground1,
		textAlign: "center",
	},
	forgotPassword: {
		textAlign: "center",
		marginTop: "16px",
	},
});

const LoginPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const setUser = useAuthStore((state) => state.setUser);
	const setToken = useAuthStore((state) => state.setToken);
	const { isAuthenticated, isLoading, checkAuth } = useCurrentUser();
	const hasChecked = useRef(false);

	const [email, setEmail] = useState(import.meta.env.VITE_DEFAULT_EMAIL || "");
	const [password, setPassword] = useState(
		import.meta.env.VITE_DEFAULT_PASSWORD || "",
	);
	const [error, setError] = useState("");

	const loginMutation = useLogin({
		mutation: {
			onSuccess: (response: loginResponse) => {
				if (response.status === 200 && response.data.token) {
					// Store the JWT token
					setToken(response.data.token);
					// Map gateway login response to MeResponse format
					setUser({
						gid: response.data.gid,
						email: response.data.email,
						firstName: response.data.firstName,
						lastName: response.data.lastName,
						roles: response.data.roles ?? [],
						isActive: true,
					});
					navigate(ROUTES.SYSTEM_USERS_LIST);
				} else if (response.status === 400) {
					setError(response.data.message || "Nieprawidłowy email lub hasło");
				}
			},
			onError: (err: Error) => {
				setError(err.message || "Nieprawidłowy email lub hasło");
			},
		},
	});

	// Sprawdź sesję przy wejściu na stronę logowania
	useEffect(() => {
		if (!hasChecked.current) {
			hasChecked.current = true;
			checkAuth();
		}
	}, [checkAuth]);

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		setError("");
		loginMutation.mutate({ data: { email, password } });
	};

	// Jeśli użytkownik jest zalogowany, przekieruj do dashboardu
	if (isAuthenticated) {
		return <Navigate to={ROUTES.SYSTEM_USERS_LIST} replace />;
	}

	// Pokaż spinner podczas sprawdzania sesji
	if (isLoading) {
		return (
			<div className={styles.container}>
				<Spinner size="large" label="Sprawdzanie sesji..." />
			</div>
		);
	}

	return (
		<div className={styles.container}>
			<Card className={styles.card}>
				<div className={styles.header}>
					<Title1>{import.meta.env.VITE_APP_NAME || "ERP by Agent"}</Title1>
					<Text className={styles.subtitle} block>
						Zaloguj się do systemu
					</Text>
				</div>

				<form onSubmit={handleSubmit} className={styles.form}>
					<Field label="Email" required>
						<Input
							type="email"
							value={email}
							onChange={(_, data) => setEmail(data.value)}
							contentBefore={<PersonRegular />}
							placeholder="Wprowadź email"
							disabled={loginMutation.isPending}
						/>
					</Field>

					<Field label="Hasło" required>
						<Input
							type="password"
							value={password}
							onChange={(_, data) => setPassword(data.value)}
							contentBefore={<LockClosedRegular />}
							placeholder="Wprowadź hasło"
							disabled={loginMutation.isPending}
						/>
					</Field>

					{error && (
						<Text className={styles.error} block>
							{error}
						</Text>
					)}

					<Button
						type="submit"
						appearance="primary"
						disabled={loginMutation.isPending}
						icon={loginMutation.isPending ? <Spinner size="tiny" /> : undefined}
					>
						{loginMutation.isPending ? "Logowanie..." : "Zaloguj się"}
					</Button>
				</form>

				<div className={styles.forgotPassword}>
					<Link to={"/todo-password-reset"}>Nie pamiętasz hasła?</Link>
				</div>
			</Card>
		</div>
	);
};

export default LoginPage;
