import {
	Button,
	DrawerBody,
	DrawerHeader,
	DrawerHeaderTitle,
	makeStyles,
	OverlayDrawer,
	Spinner,
	tokens,
} from "@fluentui/react-components";
import { DismissRegular } from "@fluentui/react-icons";
import { forwardRef, useCallback, useImperativeHandle, useState } from "react";
import {
	useCreateAppUser,
	useGetAppUser,
	useUpdateAppUser,
} from "../../../api/identity/endpoints/app-users/app-users";
import type { AppUserResponse } from "../../../api/identity/models";
import SystemUserForm, { type SystemUserFormData } from "./SystemUserForm";
import type { SystemUserFormCommand } from "./SystemUserFormCommand";

const useStyles = makeStyles({
	drawer: {
		width: "900px",
		maxWidth: "100vw",
		"@media (max-width: 960px)": {
			width: "100vw",
		},
	},
	headerActions: {
		display: "flex",
		gap: tokens.spacingHorizontalXS,
	},
	body: {
		display: "flex",
		flexDirection: "column",
		padding: tokens.spacingVerticalL,
		overflowY: "auto",
	},
	loadingContainer: {
		display: "flex",
		justifyContent: "center",
		alignItems: "center",
		padding: tokens.spacingVerticalXXL,
	},
	errorContainer: {
		padding: tokens.spacingVerticalL,
		textAlign: "center",
		color: tokens.colorPaletteRedForeground1,
	},
});

export interface SystemUserFormPanelProps {
	onSuccess?: () => void;
}

const SystemUserFormPanel = forwardRef<
	SystemUserFormCommand,
	SystemUserFormPanelProps
>(({ onSuccess }, ref) => {
	const styles = useStyles();

	const [isOpen, setIsOpen] = useState(false);
	const [mode, setMode] = useState<"create" | "edit">("create");
	const [editingGid, setEditingGid] = useState<string | null>(null);
	const [editingUser, setEditingUser] = useState<AppUserResponse | null>(null);

	const getAppUserMutation = useGetAppUser();
	const createAppUserMutation = useCreateAppUser();
	const updateAppUserMutation = useUpdateAppUser();

	useImperativeHandle(ref, () => ({
		create: () => {
			setMode("create");
			setEditingGid(null);
			setEditingUser(null);
			setIsOpen(true);
		},
		edit: (gid) => {
			setMode("edit");
			setEditingGid(gid);
			setEditingUser(null);
			setIsOpen(true);

			getAppUserMutation.mutate(
				{ data: { gid } },
				{
					onSuccess: (response) => {
						if (response.status === 200) {
							setEditingUser(response.data);
						}
					},
				},
			);
		},
	}));

	const handleClose = useCallback(() => {
		setIsOpen(false);
		setEditingGid(null);
		setEditingUser(null);
	}, []);

	const handleSubmit = useCallback(
		(data: SystemUserFormData) => {
			if (mode === "create") {
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
							if (response.status === 201) {
								handleClose();
								onSuccess?.();
							}
						},
					},
				);
			} else if (mode === "edit" && editingGid) {
				updateAppUserMutation.mutate(
					{
						gid: editingGid,
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
						onSuccess: (response) => {
							if (response.status === 200) {
								handleClose();
								onSuccess?.();
							}
						},
					},
				);
			}
		},
		[
			mode,
			editingGid,
			createAppUserMutation,
			updateAppUserMutation,
			handleClose,
			onSuccess,
		],
	);

	const isLoading =
		createAppUserMutation.isPending || updateAppUserMutation.isPending;
	const isLoadingUser = getAppUserMutation.isPending;

	const getInitialData = (): Partial<SystemUserFormData> | undefined => {
		if (mode === "edit" && editingUser) {
			return {
				firstName: editingUser.firstName ?? "",
				lastName: editingUser.lastName ?? "",
				email: editingUser.email ?? "",
				department: editingUser.department,
				roles: editingUser.roles ?? [],
				isActive: editingUser.isActive ?? true,
			};
		}
		return undefined;
	};

	return (
		<OverlayDrawer
			position="end"
			open={isOpen}
			onOpenChange={(_, data) => setIsOpen(data.open)}
			className={styles.drawer}
		>
			<DrawerHeader>
				<DrawerHeaderTitle
					action={
						<div className={styles.headerActions}>
							<Button
								appearance="subtle"
								icon={<DismissRegular />}
								onClick={handleClose}
								title="Zamknij"
							/>
						</div>
					}
				>
					{mode === "create" ? "Nowy użytkownik systemu" : "Edytuj użytkownika"}
				</DrawerHeaderTitle>
			</DrawerHeader>

			<DrawerBody className={styles.body}>
				{isLoadingUser && mode === "edit" && (
					<div className={styles.loadingContainer}>
						<Spinner size="medium" label="Ładowanie danych użytkownika..." />
					</div>
				)}

				{getAppUserMutation.isError && mode === "edit" && (
					<div className={styles.errorContainer}>
						Nie udało się pobrać danych użytkownika.
					</div>
				)}

				{(mode === "create" || (mode === "edit" && editingUser)) && (
					<SystemUserForm
						key={`${mode}-${editingGid ?? "new"}`}
						initialData={getInitialData()}
						onSubmit={handleSubmit}
						onCancel={handleClose}
						isLoading={isLoading}
						submitLabel={mode === "create" ? "Utwórz" : "Zapisz"}
						isEditMode={mode === "edit"}
					/>
				)}
			</DrawerBody>
		</OverlayDrawer>
	);
});

SystemUserFormPanel.displayName = "SystemUserFormPanel";

export default SystemUserFormPanel;
