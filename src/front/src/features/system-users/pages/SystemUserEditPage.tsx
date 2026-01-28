import { makeStyles, tokens } from "@fluentui/react-components";
import type React from "react";
import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";
import {
	useGetAppUser,
	useUpdateAppUser,
} from "../../../api/identity/endpoints/app-users/app-users";
import { Department } from "../../../api/identity/models";
import {
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import { SystemUserForm, type SystemUserFormData } from "../components";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		height: "100%",
	},
	content: {
		flex: 1,
		padding: tokens.spacingHorizontalL,
		paddingTop: tokens.spacingVerticalM,
		overflowY: "auto",
	},
});

const SystemUserEditPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const { gid } = useParams<{ gid: string }>();
	const [initialData, setInitialData] =
		useState<Partial<SystemUserFormData> | null>(null);

	const getAppUserMutation = useGetAppUser();
	const updateAppUserMutation = useUpdateAppUser();

	useEffect(() => {
		if (gid) {
			getAppUserMutation.mutate({ data: { gid } });
		}
	}, [gid]);

	useEffect(() => {
		if (
			getAppUserMutation.isSuccess &&
			getAppUserMutation.data?.status === 200
		) {
			const user = getAppUserMutation.data.data;
			setInitialData({
				firstName: user.firstName ?? "",
				lastName: user.lastName ?? "",
				email: user.email ?? "",
				department: user.department ?? Department.Sales,
				roles: user.roles ?? [],
				isActive: user.isActive ?? true,
			});
		}
	}, [getAppUserMutation.isSuccess, getAppUserMutation.data]);

	const handleSubmit = useCallback(
		(data: SystemUserFormData) => {
			if (!gid) return;

			updateAppUserMutation.mutate(
				{
					gid,
					data: {
						firstName: data.firstName,
						lastName: data.lastName,
						email: data.email,
						department: data.department,
						roles: data.roles,
						isActive: data.isActive,
					},
				},
				{
					onSuccess: () => {
						navigate(`/system-users/${gid}`);
					},
				},
			);
		},
		[gid, updateAppUserMutation, navigate],
	);

	const handleCancel = useCallback(() => {
		if (gid) {
			navigate(`/system-users/${gid}`);
		} else {
			navigate(ROUTES.SYSTEM_USERS_LIST);
		}
	}, [gid, navigate]);

	const getFullName = () => {
		if (getAppUserMutation.data?.status === 200) {
			const user = getAppUserMutation.data.data;
			const parts = [user.firstName, user.lastName].filter(Boolean);
			return parts.length > 0 ? parts.join(" ") : "Użytkownik";
		}
		return "Użytkownik";
	};

	if (getAppUserMutation.isPending) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Edycja użytkownika"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Użytkownicy systemu", href: ROUTES.SYSTEM_USERS_LIST },
						{ label: "Ładowanie..." },
					]}
				/>
				<div className={styles.content}>
					<LoadingSpinner label="Ładowanie danych użytkownika..." />
				</div>
			</div>
		);
	}

	if (getAppUserMutation.isError || getAppUserMutation.data?.status !== 200) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Edycja użytkownika"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Użytkownicy systemu", href: ROUTES.SYSTEM_USERS_LIST },
						{ label: "Błąd" },
					]}
				/>
				<div className={styles.content}>
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać danych użytkownika."
						onRetry={() => gid && getAppUserMutation.mutate({ data: { gid } })}
					/>
				</div>
			</div>
		);
	}

	const userName = getFullName();

	return (
		<div className={styles.container}>
			<PageHeader
				title={`Edycja: ${userName}`}
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Użytkownicy systemu", href: ROUTES.SYSTEM_USERS_LIST },
					{ label: userName, href: `/system-users/${gid}` },
					{ label: "Edycja" },
				]}
			/>
			<div className={styles.content}>
				{initialData && (
					<SystemUserForm
						initialData={initialData}
						onSubmit={handleSubmit}
						onCancel={handleCancel}
						isLoading={updateAppUserMutation.isPending}
						submitLabel="Zapisz zmiany"
						isEditMode={true}
					/>
				)}
			</div>
		</div>
	);
};

export default SystemUserEditPage;
