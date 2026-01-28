import { makeStyles, tokens } from "@fluentui/react-components";
import type React from "react";
import { useCallback } from "react";
import { useNavigate } from "react-router";
import { useCreateAppUser } from "../../../api/identity/endpoints/app-users/app-users";
import { PageHeader } from "../../../components/common";
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

const SystemUserCreatePage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const createAppUserMutation = useCreateAppUser();

	const handleSubmit = useCallback(
		(data: SystemUserFormData) => {
			createAppUserMutation.mutate(
				{
					data: {
						firstName: data.firstName,
						lastName: data.lastName,
						email: data.email,
						password: data.password,
						department: data.department,
						roles: data.roles,
					},
				},
				{
					onSuccess: (response) => {
						if (response.status === 201 && response.data.gid) {
							navigate(`/system-users/${response.data.gid}`);
						} else {
							navigate(ROUTES.SYSTEM_USERS_LIST);
						}
					},
				},
			);
		},
		[createAppUserMutation, navigate],
	);

	const handleCancel = useCallback(() => {
		navigate(ROUTES.SYSTEM_USERS_LIST);
	}, [navigate]);

	return (
		<div className={styles.container}>
			<PageHeader
				title="Nowy użytkownik systemu"
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Użytkownicy systemu", href: ROUTES.SYSTEM_USERS_LIST },
					{ label: "Nowy użytkownik" },
				]}
			/>
			<div className={styles.content}>
				<SystemUserForm
					onSubmit={handleSubmit}
					onCancel={handleCancel}
					isLoading={createAppUserMutation.isPending}
					submitLabel="Utwórz użytkownika"
					isEditMode={false}
				/>
			</div>
		</div>
	);
};

export default SystemUserCreatePage;
