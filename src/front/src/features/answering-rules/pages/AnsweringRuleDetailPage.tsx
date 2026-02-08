import {
	Badge,
	Button,
	Card,
	Divider,
	Table,
	TableBody,
	TableCell,
	TableHeader,
	TableHeaderCell,
	TableRow,
	Text,
	makeStyles,
	tokens,
} from "@fluentui/react-components";
import {
	ArrowLeftRegular,
	CalendarRegular,
	DeleteRegular,
	EditRegular,
	MailRegular,
	ToggleLeftRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { useCallback, useState } from "react";
import { useNavigate, useParams } from "react-router";

import {
	useDeleteRule,
	useGetRuleByGid,
	useToggleRule,
} from "../../../api/answerrule/endpoints/answering-rules/answering-rules";
import { DayOfWeek } from "../../../api/answerrule/models";
import {
	ConfirmDialog,
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import { ActionTypeBadge } from "../components";

const DAY_OF_WEEK_LABELS: Record<DayOfWeek, string> = {
	Monday: "Poniedziałek",
	Tuesday: "Wtorek",
	Wednesday: "Środa",
	Thursday: "Czwartek",
	Friday: "Piątek",
	Saturday: "Sobota",
	Sunday: "Niedziela",
};

const useStyles = makeStyles({
	container: { display: "flex", flexDirection: "column", height: "100%" },
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
		"@media (max-width: 768px)": { gridTemplateColumns: "1fr" },
	},
	card: { padding: tokens.spacingVerticalL },
	cardFullWidth: {
		padding: tokens.spacingVerticalL,
		gridColumn: "1 / -1",
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
	section: { marginTop: tokens.spacingVerticalL },
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
	detailIcon: { color: tokens.colorNeutralForeground3, flexShrink: 0 },
	detailLabel: { color: tokens.colorNeutralForeground3, minWidth: "120px" },
	actions: {
		display: "flex",
		flexWrap: "wrap",
		gap: tokens.spacingHorizontalS,
		marginTop: tokens.spacingVerticalL,
	},
	timeSlotsTable: { width: "100%" },
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

const StatusBadge: React.FC<{ isEnabled?: boolean }> = ({ isEnabled }) => (
	<Badge appearance="tint" color={isEnabled ? "success" : "danger"}>
		{isEnabled ? "Aktywna" : "Nieaktywna"}
	</Badge>
);

const AnsweringRuleDetailPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const { gid } = useParams<{ gid: string }>();
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);

	const ruleQuery = useGetRuleByGid(gid ?? "");
	const deleteRuleMutation = useDeleteRule();
	const toggleRuleMutation = useToggleRule();

	const rule = ruleQuery.data?.status === 200 ? ruleQuery.data.data : null;

	const handleEdit = () => {
		if (gid) navigate(`/answering-rules/${gid}/edit`);
	};

	const handleDelete = useCallback(() => {
		if (!gid) return;
		deleteRuleMutation.mutate(
			{ gid },
			{ onSuccess: () => navigate(ROUTES.ANSWERING_RULES_LIST) },
		);
	}, [gid, deleteRuleMutation, navigate]);

	const handleToggle = useCallback(() => {
		if (!gid) return;
		toggleRuleMutation.mutate(
			{ gid },
			{ onSuccess: () => ruleQuery.refetch() },
		);
	}, [gid, toggleRuleMutation, ruleQuery]);

	if (ruleQuery.isLoading) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Szczegóły reguły"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{
							label: "Reguły odpowiadania",
							href: ROUTES.ANSWERING_RULES_LIST,
						},
						{ label: "Ładowanie..." },
					]}
				/>
				<div className={styles.content}>
					<LoadingSpinner label="Ładowanie danych reguły..." />
				</div>
			</div>
		);
	}

	if (ruleQuery.isError || !rule) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Szczegóły reguły"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{
							label: "Reguły odpowiadania",
							href: ROUTES.ANSWERING_RULES_LIST,
						},
						{ label: "Błąd" },
					]}
				/>
				<div className={styles.content}>
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać danych reguły."
						onRetry={() => ruleQuery.refetch()}
					/>
				</div>
			</div>
		);
	}

	const sortedTimeSlots = [...(rule.timeSlots ?? [])].sort((a, b) => {
		const dayOrder = Object.values(DayOfWeek);
		const dayA = dayOrder.indexOf(a.dayOfWeek as DayOfWeek);
		const dayB = dayOrder.indexOf(b.dayOfWeek as DayOfWeek);
		if (dayA !== dayB) return dayA - dayB;
		return (a.startTime ?? "").localeCompare(b.startTime ?? "");
	});

	return (
		<div className={styles.container}>
			<PageHeader
				title={rule.name || "Reguła"}
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{
						label: "Reguły odpowiadania",
						href: ROUTES.ANSWERING_RULES_LIST,
					},
					{ label: rule.name || "Szczegóły" },
				]}
				actions={
					<Button
						appearance="subtle"
						icon={<ArrowLeftRegular />}
						onClick={() => navigate(ROUTES.ANSWERING_RULES_LIST)}
					>
						Powrót do listy
					</Button>
				}
			/>

			<div className={styles.content}>
				<div className={styles.grid}>
					{/* Informacje ogolne */}
					<Card className={styles.card}>
						<div className={styles.header}>
							<div className={styles.headerInfo}>
								<Text className={styles.headerName}>
									{rule.name || "Brak nazwy"}
								</Text>
								<div
									style={{
										display: "flex",
										gap: "8px",
										alignItems: "center",
									}}
								>
									<StatusBadge isEnabled={rule.isEnabled} />
									<Badge appearance="outline">Priorytet: {rule.priority}</Badge>
								</div>
							</div>
						</div>

						<Divider />

						<div className={styles.section}>
							<Text className={styles.sectionTitle}>Akcja</Text>
							<div className={styles.detailRow}>
								<Text className={styles.detailLabel}>Typ:</Text>
								{rule.actionType && (
									<ActionTypeBadge actionType={rule.actionType} />
								)}
							</div>
							{rule.actionTarget && (
								<div className={styles.detailRow}>
									<Text className={styles.detailLabel}>Cel:</Text>
									<Text>{rule.actionTarget}</Text>
								</div>
							)}
							{rule.voicemailBoxGid && (
								<div className={styles.detailRow}>
									<Text className={styles.detailLabel}>Voicemail:</Text>
									<Text style={{ fontFamily: "monospace", fontSize: "12px" }}>
										{rule.voicemailBoxGid}
									</Text>
								</div>
							)}
							{rule.voiceMessageGid && (
								<div className={styles.detailRow}>
									<Text className={styles.detailLabel}>Komunikat:</Text>
									<Text style={{ fontFamily: "monospace", fontSize: "12px" }}>
										{rule.voiceMessageGid}
									</Text>
								</div>
							)}
						</div>

						{rule.description && (
							<div className={styles.section}>
								<Text className={styles.sectionTitle}>Opis</Text>
								<Text>{rule.description}</Text>
							</div>
						)}

						<div className={styles.section}>
							<Text className={styles.sectionTitle}>Notyfikacje</Text>
							<div className={styles.detailRow}>
								<MailRegular className={styles.detailIcon} />
								<Text className={styles.detailLabel}>Email:</Text>
								<Text>
									{rule.sendEmailNotification
										? rule.notificationEmail || "Domyślny"
										: "Wyłączone"}
								</Text>
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
								icon={<ToggleLeftRegular />}
								onClick={handleToggle}
								disabled={toggleRuleMutation.isPending}
							>
								{rule.isEnabled ? "Wyłącz" : "Włącz"}
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

					{/* Informacje systemowe */}
					<Card className={styles.card}>
						<Text className={styles.sectionTitle}>Informacje systemowe</Text>
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>Konto SIP:</Text>
							<Text style={{ fontFamily: "monospace", fontSize: "12px" }}>
								{rule.sipAccountGid || "-"}
							</Text>
						</div>
						<div className={styles.detailRow}>
							<CalendarRegular className={styles.detailIcon} />
							<Text className={styles.detailLabel}>Utworzono:</Text>
							<Text>{formatDate(rule.createdAt)}</Text>
						</div>
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>GID:</Text>
							<Text style={{ fontFamily: "monospace", fontSize: "12px" }}>
								{rule.gid}
							</Text>
						</div>
					</Card>

					{/* Przedzialy czasowe */}
					<Card className={styles.cardFullWidth}>
						<Text className={styles.sectionTitle}>
							Przedziały czasowe ({sortedTimeSlots.length})
						</Text>
						{sortedTimeSlots.length > 0 ? (
							<Table
								size="small"
								className={styles.timeSlotsTable}
								aria-label="Przedziały czasowe"
							>
								<TableHeader>
									<TableRow>
										<TableHeaderCell>Dzień</TableHeaderCell>
										<TableHeaderCell>Od</TableHeaderCell>
										<TableHeaderCell>Do</TableHeaderCell>
										<TableHeaderCell>Cały dzień</TableHeaderCell>
									</TableRow>
								</TableHeader>
								<TableBody>
									{sortedTimeSlots.map((slot, idx) => (
										<TableRow
											key={`${slot.dayOfWeek}-${slot.startTime}-${idx}`}
										>
											<TableCell>
												{DAY_OF_WEEK_LABELS[slot.dayOfWeek as DayOfWeek] ??
													slot.dayOfWeek}
											</TableCell>
											<TableCell>
												{slot.isAllDay ? "-" : slot.startTime}
											</TableCell>
											<TableCell>
												{slot.isAllDay ? "-" : slot.endTime}
											</TableCell>
											<TableCell>{slot.isAllDay ? "Tak" : "Nie"}</TableCell>
										</TableRow>
									))}
								</TableBody>
							</Table>
						) : (
							<Text
								style={{
									color: tokens.colorNeutralForeground3,
									fontStyle: "italic",
								}}
							>
								Brak przedziałów czasowych
							</Text>
						)}
					</Card>
				</div>
			</div>

			<ConfirmDialog
				open={showDeleteDialog}
				onOpenChange={(open) => setShowDeleteDialog(open)}
				title="Usuń regułę"
				message={`Czy na pewno chcesz usunąć regułę "${rule.name || "bez nazwy"}"? Tej operacji nie można cofnąć.`}
				confirmLabel="Usuń"
				cancelLabel="Anuluj"
				onConfirm={handleDelete}
				danger
				loading={deleteRuleMutation.isPending}
			/>
		</div>
	);
};

export default AnsweringRuleDetailPage;
