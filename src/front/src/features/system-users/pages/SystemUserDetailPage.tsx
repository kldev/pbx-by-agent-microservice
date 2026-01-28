import {
	Avatar,
	Button,
	Card,
	Divider,
	makeStyles,
	Text,
	tokens,
} from "@fluentui/react-components";
import {
	ArrowLeftRegular,
	CalendarRegular,
	DeleteRegular,
	EditRegular,
	KeyRegular,
	MailRegular,
	PeopleTeamRegular,
	ToggleLeftRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { useCallback, useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router";
import {
	useDeleteAppUser,
	useGetAppUser,
	useUpdateAppUser,
} from "../../../api/identity/endpoints/app-users/app-users";
import { useGetTeamList } from "../../../api/identity/endpoints/teams/teams";
import type { AppUserResponse } from "../../../api/identity/models";
import {
	ConfirmDialog,
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import {
	ChangePasswordDialog,
	ChangeTeamDialog,
	SystemUserDepartmentBadge,
	SystemUserFormPanel,
	SystemUserRolesBadge,
	SystemUserStatusBadge,
} from "../components";
import type { ChangeTeamDialogCommand } from "../components/ChangeTeamDialogCommand";
import type { SystemUserFormCommand } from "../components/SystemUserFormCommand";

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
	grid: {
		display: "grid",
		gridTemplateColumns: "1fr 1fr",
		gap: tokens.spacingHorizontalL,
		maxWidth: "1000px",
		"@media (max-width: 768px)": {
			gridTemplateColumns: "1fr",
		},
	},
	card: {
		padding: tokens.spacingVerticalL,
	},
	profileHeader: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalL,
		marginBottom: tokens.spacingVerticalL,
	},
	profileInfo: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXS,
	},
	profileName: {
		fontSize: tokens.fontSizeBase500,
		fontWeight: tokens.fontWeightSemibold,
	},
	section: {
		marginTop: tokens.spacingVerticalL,
	},
	sectionTitle: {
		fontSize: tokens.fontSizeBase400,
		fontWeight: tokens.fontWeightSemibold,
		marginBottom: tokens.spacingVerticalM,
		color: tokens.colorNeutralForeground1,
	},
	detailRow: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
		marginBottom: tokens.spacingVerticalS,
		color: tokens.colorNeutralForeground2,
	},
	detailIcon: {
		color: tokens.colorNeutralForeground3,
		flexShrink: 0,
	},
	detailLabel: {
		color: tokens.colorNeutralForeground3,
		minWidth: "100px",
	},
	actions: {
		display: "flex",
		flexWrap: "wrap",
		gap: tokens.spacingHorizontalS,
		marginTop: tokens.spacingVerticalL,
	},
	fullWidth: {
		gridColumn: "1 / -1",
	},
});

function formatDate(dateString?: string | null): string {
	if (!dateString) return "-";
	return new Date(dateString).toLocaleDateString("pl-PL", {
		year: "numeric",
		month: "long",
		day: "numeric",
		hour: "2-digit",
		minute: "2-digit",
	});
}

function getInitials(
	firstName?: string | null,
	lastName?: string | null,
): string {
	const first = firstName?.[0]?.toUpperCase() || "";
	const last = lastName?.[0]?.toUpperCase() || "";
	return first + last || "?";
}

function getFullName(user: AppUserResponse): string {
	const parts = [user.firstName, user.lastName].filter(Boolean);
	return parts.length > 0 ? parts.join(" ") : "Brak nazwy";
}

const SystemUserDetailPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const { gid } = useParams<{ gid: string }>();
	const systemUserFormRef = useRef<SystemUserFormCommand>(null);
	const changeTeamDialogRef = useRef<ChangeTeamDialogCommand>(null);
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);
	const [showPasswordDialog, setShowPasswordDialog] = useState(false);
	const [teamName, setTeamName] = useState<string | null>(null);

	const getAppUserMutation = useGetAppUser();
	const deleteAppUserMutation = useDeleteAppUser();
	const updateAppUserMutation = useUpdateAppUser();
	const getTeamListMutation = useGetTeamList();

	const refreshUser = useCallback(() => {
		if (gid) {
			getAppUserMutation.mutate({ data: { gid } });
		}
	}, [gid, getAppUserMutation]);

	useEffect(() => {
		if (gid) {
			getAppUserMutation.mutate({ data: { gid } });
		}
	}, [gid]);

	const user =
		getAppUserMutation.data?.status === 200
			? getAppUserMutation.data.data
			: null;

	useEffect(() => {
		if (user?.teamId) {
			getTeamListMutation.mutate({
				data: {
					pageNumber: 1,
					pageSize: 100,
				},
			});
		} else {
			setTeamName(null);
		}
	}, [user?.teamId]);

	useEffect(() => {
		if (getTeamListMutation.data?.status === 200 && user?.teamId) {
			const team = getTeamListMutation.data.data.items?.find(
				(t) => t.id === user.teamId,
			);
			setTeamName(team?.name ?? null);
		}
	}, [getTeamListMutation.data, user?.teamId]);

	const handleEdit = () => {
		if (gid) {
			systemUserFormRef.current?.edit(gid);
		}
	};

	const handleDelete = useCallback(() => {
		if (!gid) return;
		deleteAppUserMutation.mutate(
			{ gid },
			{
				onSuccess: () => {
					navigate(ROUTES.SYSTEM_USERS_LIST);
				},
			},
		);
	}, [gid, deleteAppUserMutation, navigate]);

	const handleToggleActive = useCallback(() => {
		if (!gid || !user) return;
		updateAppUserMutation.mutate(
			{
				gid,
				data: {
					firstName: user.firstName ?? "",
					lastName: user.lastName ?? "",
					email: user.email ?? "",
					isActive: !user.isActive,
				},
			},
			{
				onSuccess: () => {
					getAppUserMutation.mutate({ data: { gid } });
				},
			},
		);
	}, [gid, user, updateAppUserMutation, getAppUserMutation]);

	const handleBack = () => {
		navigate(ROUTES.SYSTEM_USERS_LIST);
	};

	const handlePasswordSuccess = () => {
		if (gid) {
			getAppUserMutation.mutate({ data: { gid } });
		}
	};

	if (getAppUserMutation.isPending) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Szczegóły użytkownika"
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

	if (getAppUserMutation.isError || !user) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Szczegóły użytkownika"
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

	const fullName = getFullName(user);

	return (
		<div className={styles.container}>
			<PageHeader
				title={fullName}
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Użytkownicy systemu", href: ROUTES.SYSTEM_USERS_LIST },
					{ label: fullName },
				]}
				actions={
					<Button
						appearance="subtle"
						icon={<ArrowLeftRegular />}
						onClick={handleBack}
					>
						Powrót do listy
					</Button>
				}
			/>
			<div className={styles.content}>
				<div className={styles.grid}>
					<Card className={styles.card}>
						<div className={styles.profileHeader}>
							<Avatar
								name={fullName}
								initials={getInitials(user.firstName, user.lastName)}
								size={72}
							/>
							<div className={styles.profileInfo}>
								<Text className={styles.profileName}>{fullName}</Text>
								<div
									style={{ display: "flex", gap: "8px", alignItems: "center" }}
								>
									<SystemUserStatusBadge isActive={user.isActive ?? false} />
									<SystemUserDepartmentBadge department={user.department} />
								</div>
							</div>
						</div>

						<Divider />

						<div className={styles.section}>
							<Text className={styles.sectionTitle}>Dane kontaktowe</Text>
							{user.email && (
								<div className={styles.detailRow}>
									<MailRegular className={styles.detailIcon} />
									<Text>{user.email}</Text>
								</div>
							)}
						</div>

						<div className={styles.section}>
							<Text className={styles.sectionTitle}>Uprawnienia</Text>
							<SystemUserRolesBadge roles={user.roles} />
						</div>

						<div className={styles.section}>
							<Text className={styles.sectionTitle}>Organizacja</Text>
							<div className={styles.detailRow}>
								<PeopleTeamRegular className={styles.detailIcon} />
								<Text className={styles.detailLabel}>Zespół:</Text>
								<Text>{teamName ?? "Brak zespołu"}</Text>
								<Button
									appearance="subtle"
									size="small"
									icon={<EditRegular />}
									onClick={() =>
										user && changeTeamDialogRef.current?.open(user)
									}
								>
									Zmień
								</Button>
							</div>
						</div>

						<div className={styles.actions}>
							<Button
								appearance="primary"
								icon={<EditRegular />}
								onClick={handleEdit}
							>
								Edytuj
							</Button>
							<Button
								appearance="secondary"
								icon={<KeyRegular />}
								onClick={() => setShowPasswordDialog(true)}
							>
								Zmień hasło
							</Button>
							<Button
								appearance="secondary"
								icon={<ToggleLeftRegular />}
								onClick={handleToggleActive}
								disabled={updateAppUserMutation.isPending}
							>
								{user.isActive ? "Dezaktywuj" : "Aktywuj"}
							</Button>
							<Button
								appearance="secondary"
								icon={<DeleteRegular />}
								onClick={() => setShowDeleteDialog(true)}
							>
								Usuń
							</Button>
						</div>
					</Card>

					<Card className={styles.card}>
						<Text className={styles.sectionTitle}>Informacje systemowe</Text>
						<div className={styles.detailRow}>
							<CalendarRegular className={styles.detailIcon} />
							<Text className={styles.detailLabel}>Utworzono:</Text>
							<Text>{formatDate(user.createdAt)}</Text>
						</div>
						{user.modifiedAt && (
							<div className={styles.detailRow}>
								<CalendarRegular className={styles.detailIcon} />
								<Text className={styles.detailLabel}>Zmodyfikowano:</Text>
								<Text>{formatDate(user.modifiedAt)}</Text>
							</div>
						)}
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>GID:</Text>
							<Text style={{ fontFamily: "monospace", fontSize: "12px" }}>
								{user.gid}
							</Text>
						</div>
					</Card>
				</div>
			</div>

			<ConfirmDialog
				open={showDeleteDialog}
				onOpenChange={(open) => setShowDeleteDialog(open)}
				title="Usuń użytkownika"
				message={`Czy na pewno chcesz usunąć użytkownika "${fullName}"? Tej operacji nie można cofnąć.`}
				confirmLabel="Usuń"
				cancelLabel="Anuluj"
				onConfirm={handleDelete}
				danger
				loading={deleteAppUserMutation.isPending}
			/>

			<ChangePasswordDialog
				open={showPasswordDialog}
				employeeGid={gid || null}
				employeeName={fullName}
				onOpenChange={(open) => setShowPasswordDialog(open)}
				onSuccess={handlePasswordSuccess}
			/>

			<SystemUserFormPanel ref={systemUserFormRef} onSuccess={refreshUser} />

			<ChangeTeamDialog ref={changeTeamDialogRef} onSuccess={refreshUser} />
		</div>
	);
};

export default SystemUserDetailPage;
