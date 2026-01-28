import {
	Badge,
	Button,
	Card,
	Divider,
	makeStyles,
	Text,
	tokens,
} from "@fluentui/react-components";
import {
	ArrowLeftRegular,
	BuildingRegular,
	CalendarRegular,
	DeleteRegular,
	EditRegular,
	ToggleLeftRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";

import {
	ConfirmDialog,
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import { TeamStatusBadge } from "../components";
import {
	useDeleteTeam,
	useGetTeam,
	useUpdateTeam,
} from "../../../api/identity/endpoints/teams/teams";

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
	header: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalM,
		marginBottom: tokens.spacingVerticalL,
	},
	headerInfo: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXS,
	},
	headerName: {
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

const TeamDetailPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const { gid } = useParams<{ gid: string }>();
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);

	const teamMutation = useGetTeam();
	const deleteTeamMutation = useDeleteTeam();
	const updateTeamMutation = useUpdateTeam();

	useEffect(() => {
		if (gid) {
			teamMutation.mutate({ data: { gid } });
		}
	}, [gid]);

	const team =
		teamMutation.data?.status === 200 ? teamMutation.data.data : null;

	const handleEdit = () => {
		if (gid) {
			navigate(`/teams/${gid}/edit`);
		}
	};

	const handleDelete = useCallback(() => {
		if (!gid) return;
		deleteTeamMutation.mutate(
			{ gid },
			{
				onSuccess: () => {
					navigate(ROUTES.TEAMS_LIST);
				},
			},
		);
	}, [gid, deleteTeamMutation, navigate]);

	const handleToggleActive = useCallback(() => {
		if (!gid || !team) return;
		updateTeamMutation.mutate(
			{
				gid,
				data: {
					code: team.code ?? "",
					name: team.name ?? "",
					structureId: team.structureId,
					isActive: !team.isActive,
				},
			},
			{
				onSuccess: () => {
					if (gid) {
						teamMutation.mutate({ data: { gid } });
					}
				},
			},
		);
	}, [gid, team, updateTeamMutation, teamMutation]);

	const handleBack = () => {
		navigate(ROUTES.TEAMS_LIST);
	};

	if (teamMutation.isPending) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Szczegóły zespołu"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Zespoły", href: ROUTES.TEAMS_LIST },
						{ label: "Ładowanie..." },
					]}
				/>
				<div className={styles.content}>
					<LoadingSpinner label="Ładowanie danych zespołu..." />
				</div>
			</div>
		);
	}

	if (teamMutation.isError || !team) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Szczegóły zespołu"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Zespoły", href: ROUTES.TEAMS_LIST },
						{ label: "Błąd" },
					]}
				/>
				<div className={styles.content}>
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać danych zespołu."
						onRetry={() => {
							if (gid) {
								teamMutation.mutate({ data: { gid } });
							}
						}}
					/>
				</div>
			</div>
		);
	}

	return (
		<div className={styles.container}>
			<PageHeader
				title={team.name || "Zespół"}
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Zespoły", href: ROUTES.TEAMS_LIST },
					{ label: team.name || "Szczegóły" },
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
						<div className={styles.header}>
							<div className={styles.headerInfo}>
								<Text className={styles.headerName}>
									{team.name || "Brak nazwy"}
								</Text>
								<div
									style={{ display: "flex", gap: "8px", alignItems: "center" }}
								>
									<TeamStatusBadge isActive={team.isActive ?? false} />
									<Badge appearance="outline">{team.code}</Badge>
								</div>
							</div>
						</div>

						<Divider />

						<div className={styles.section}>
							<Text className={styles.sectionTitle}>Organizacja</Text>
							{team.sbuName && (
								<div className={styles.detailRow}>
									<BuildingRegular className={styles.detailIcon} />
									<Text className={styles.detailLabel}>SBU:</Text>
									<Text>{team.sbuName}</Text>
								</div>
							)}
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
								icon={<ToggleLeftRegular />}
								onClick={handleToggleActive}
								disabled={updateTeamMutation.isPending}
							>
								{team.isActive ? "Dezaktywuj" : "Aktywuj"}
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
							<Text>{formatDate(team.createdAt)}</Text>
						</div>
						{team.modifiedAt && (
							<div className={styles.detailRow}>
								<CalendarRegular className={styles.detailIcon} />
								<Text className={styles.detailLabel}>Zmodyfikowano:</Text>
								<Text>{formatDate(team.modifiedAt)}</Text>
							</div>
						)}
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>GID:</Text>
							<Text style={{ fontFamily: "monospace", fontSize: "12px" }}>
								{team.gid}
							</Text>
						</div>
					</Card>
				</div>
			</div>

			<ConfirmDialog
				open={showDeleteDialog}
				onOpenChange={(open) => setShowDeleteDialog(open)}
				title="Usuń zespół"
				message={`Czy na pewno chcesz usunąć zespół "${team.name || "bez nazwy"}"? Tej operacji nie można cofnąć.`}
				confirmLabel="Usuń"
				cancelLabel="Anuluj"
				onConfirm={handleDelete}
				danger
				loading={deleteTeamMutation.isPending}
			/>
		</div>
	);
};

export default TeamDetailPage;
