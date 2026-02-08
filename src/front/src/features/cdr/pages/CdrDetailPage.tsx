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
	CallRegular,
	CalendarRegular,
	ClockRegular,
	MoneyRegular,
	PersonRegular,
	ServerRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { useNavigate, useParams } from "react-router";

import {
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import { useGetCallRecordByGid } from "../../../api/cdr/endpoints/call-records/call-records";
import { CdrTypeBadge } from "../components";

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
		maxWidth: "1200px",
		"@media (max-width: 960px)": {
			gridTemplateColumns: "1fr",
		},
	},
	card: {
		padding: tokens.spacingVerticalL,
	},
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
		fontFamily: "monospace",
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
		minWidth: "140px",
	},
	detailValue: {
		fontFamily: "monospace",
	},
});

function formatDateTime(dateString?: string | null): string {
	if (!dateString) return "-";
	return new Date(dateString).toLocaleString("pl-PL", {
		year: "numeric",
		month: "2-digit",
		day: "2-digit",
		hour: "2-digit",
		minute: "2-digit",
		second: "2-digit",
	});
}

function formatDuration(seconds?: number): string {
	if (seconds === undefined || seconds === null) return "-";
	const mins = Math.floor(seconds / 60);
	const secs = seconds % 60;
	return `${mins}:${secs.toString().padStart(2, "0")} (${seconds}s)`;
}

function formatCost(cost?: number, currency?: string | null): string {
	if (cost === undefined || cost === null) return "-";
	const formattedCost = cost.toFixed(4);
	return currency ? `${formattedCost} ${currency}` : formattedCost;
}

const CdrDetailPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const { gid } = useParams<{ gid: string }>();

	const { data, isPending, isError, refetch } = useGetCallRecordByGid(
		gid ?? "",
		{
			query: {
				enabled: !!gid,
			},
		},
	);

	const cdr = data?.status === 200 ? data.data : null;

	const handleBack = () => {
		navigate(ROUTES.CDR_LIST);
	};

	if (isPending) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Szczegóły połączenia"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Historia połączeń", href: ROUTES.CDR_LIST },
						{ label: "Ładowanie..." },
					]}
				/>
				<div className={styles.content}>
					<LoadingSpinner label="Ładowanie danych połączenia..." />
				</div>
			</div>
		);
	}

	if (isError || !cdr) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Szczegóły połączenia"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Historia połączeń", href: ROUTES.CDR_LIST },
						{ label: "Błąd" },
					]}
				/>
				<div className={styles.content}>
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać danych połączenia."
						onRetry={() => refetch()}
					/>
				</div>
			</div>
		);
	}

	const title = `${cdr.callerId || "Nieznany"} → ${cdr.calledNumber || "Nieznany"}`;

	return (
		<div className={styles.container}>
			<PageHeader
				title="Szczegóły połączenia"
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Historia połączeń", href: ROUTES.CDR_LIST },
					{ label: title },
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
					{/* Podstawowe informacje */}
					<Card className={styles.card}>
						<div className={styles.header}>
							<CallRegular style={{ fontSize: "24px" }} />
							<div className={styles.headerInfo}>
								<Text className={styles.headerName}>{title}</Text>
								<div
									style={{ display: "flex", gap: "8px", alignItems: "center" }}
								>
									<CdrTypeBadge
										statusCode={cdr.callStatusCode ?? ""}
										statusDesc={cdr.callStatusName ?? ""}
									/>
									{cdr.callDirectionCode && (
										<Badge appearance="outline">{cdr.callDirectionCode}</Badge>
									)}
								</div>
							</div>
						</div>

						<Divider />

						<div className={styles.section}>
							<Text className={styles.sectionTitle}>Numery</Text>
							<div className={styles.detailRow}>
								<Text className={styles.detailLabel}>Z numeru:</Text>
								<Text className={styles.detailValue}>
									{cdr.callerId || "-"}
								</Text>
							</div>
							<div className={styles.detailRow}>
								<Text className={styles.detailLabel}>Na numer:</Text>
								<Text className={styles.detailValue}>
									{cdr.calledNumber || "-"}
								</Text>
							</div>
							{cdr.destinationName && (
								<div className={styles.detailRow}>
									<Text className={styles.detailLabel}>Kierunek:</Text>
									<Text>{cdr.destinationName}</Text>
								</div>
							)}
							{cdr.matchedPrefix && (
								<div className={styles.detailRow}>
									<Text className={styles.detailLabel}>Dopasowany prefix:</Text>
									<Text className={styles.detailValue}>
										{cdr.matchedPrefix}
									</Text>
								</div>
							)}
						</div>
					</Card>

					{/* Czas połączenia */}
					<Card className={styles.card}>
						<Text className={styles.sectionTitle}>Czas połączenia</Text>
						<div className={styles.detailRow}>
							<ClockRegular className={styles.detailIcon} />
							<Text className={styles.detailLabel}>Start:</Text>
							<Text>{formatDateTime(cdr.startTime)}</Text>
						</div>
						{cdr.answerTime && (
							<div className={styles.detailRow}>
								<ClockRegular className={styles.detailIcon} />
								<Text className={styles.detailLabel}>Odebranie:</Text>
								<Text>{formatDateTime(cdr.answerTime)}</Text>
							</div>
						)}
						<div className={styles.detailRow}>
							<ClockRegular className={styles.detailIcon} />
							<Text className={styles.detailLabel}>Koniec:</Text>
							<Text>{formatDateTime(cdr.endTime)}</Text>
						</div>
						<Divider style={{ margin: "12px 0" }} />
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>Czas trwania:</Text>
							<Text>{formatDuration(cdr.duration)}</Text>
						</div>
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>Czas naliczany:</Text>
							<Text>{formatDuration(cdr.billableSeconds)}</Text>
						</div>
					</Card>

					{/* Billing */}
					<Card className={styles.card}>
						<Text className={styles.sectionTitle}>Rozliczenie</Text>
						<div className={styles.detailRow}>
							<MoneyRegular className={styles.detailIcon} />
							<Text className={styles.detailLabel}>Koszt całkowity:</Text>
							<Text style={{ fontWeight: tokens.fontWeightSemibold }}>
								{formatCost(cdr.totalCost, cdr.currencyCode)}
							</Text>
						</div>
						<Divider style={{ margin: "12px 0" }} />
						{cdr.tariffName && (
							<div className={styles.detailRow}>
								<Text className={styles.detailLabel}>Taryfa:</Text>
								<Text>{cdr.tariffName}</Text>
							</div>
						)}
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>Stawka/min:</Text>
							<Text>{formatCost(cdr.ratePerMinute, cdr.currencyCode)}</Text>
						</div>
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>Opłata za połączenie:</Text>
							<Text>{formatCost(cdr.connectionFee, cdr.currencyCode)}</Text>
						</div>
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>Inkrement (sek):</Text>
							<Text>{cdr.billingIncrement ?? "-"}</Text>
						</div>
					</Card>

					{/* Status i routing */}
					<Card className={styles.card}>
						<Text className={styles.sectionTitle}>Status i routing</Text>
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>Status:</Text>
							<Text>{cdr.callStatusName || cdr.callStatusCode || "-"}</Text>
						</div>
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>Kierunek:</Text>
							<Text>{cdr.callDirectionCode || "-"}</Text>
						</div>
						{(cdr.terminationCauseCode || cdr.terminationCauseName) && (
							<div className={styles.detailRow}>
								<Text className={styles.detailLabel}>
									Przyczyna zakończenia:
								</Text>
								<Text>
									{cdr.terminationCauseName || cdr.terminationCauseCode}
								</Text>
							</div>
						)}
						<Divider style={{ margin: "12px 0" }} />
						{cdr.sourceGatewayName && (
							<div className={styles.detailRow}>
								<ServerRegular className={styles.detailIcon} />
								<Text className={styles.detailLabel}>Gateway źródłowy:</Text>
								<Text>{cdr.sourceGatewayName}</Text>
							</div>
						)}
						{cdr.destinationGatewayName && (
							<div className={styles.detailRow}>
								<ServerRegular className={styles.detailIcon} />
								<Text className={styles.detailLabel}>Gateway docelowy:</Text>
								<Text>{cdr.destinationGatewayName}</Text>
							</div>
						)}
					</Card>

					{/* Klient i konto */}
					<Card className={styles.card}>
						<Text className={styles.sectionTitle}>Klient i konto SIP</Text>
						<div className={styles.detailRow}>
							<PersonRegular className={styles.detailIcon} />
							<Text className={styles.detailLabel}>Klient:</Text>
							<Text>{cdr.customerName || "-"}</Text>
						</div>
						{cdr.sipAccountUsername && (
							<div className={styles.detailRow}>
								<Text className={styles.detailLabel}>Konto SIP:</Text>
								<Text className={styles.detailValue}>
									{cdr.sipAccountUsername}
								</Text>
							</div>
						)}
					</Card>

					{/* Informacje systemowe */}
					<Card className={styles.card}>
						<Text className={styles.sectionTitle}>Informacje systemowe</Text>
						<div className={styles.detailRow}>
							<CalendarRegular className={styles.detailIcon} />
							<Text className={styles.detailLabel}>Utworzono:</Text>
							<Text>{formatDateTime(cdr.createdAt)}</Text>
						</div>
						<div className={styles.detailRow}>
							<Text className={styles.detailLabel}>GID:</Text>
							<Text style={{ fontFamily: "monospace", fontSize: "12px" }}>
								{cdr.gid}
							</Text>
						</div>
						{cdr.callUuid && (
							<div className={styles.detailRow}>
								<Text className={styles.detailLabel}>Call UUID:</Text>
								<Text style={{ fontFamily: "monospace", fontSize: "12px" }}>
									{cdr.callUuid}
								</Text>
							</div>
						)}
					</Card>

					{/* User Data - jeśli istnieje */}
					{cdr.userData && (
						<Card className={styles.cardFullWidth}>
							<Text className={styles.sectionTitle}>Dane dodatkowe</Text>
							<Text
								style={{
									fontFamily: "monospace",
									fontSize: "12px",
									whiteSpace: "pre-wrap",
									wordBreak: "break-all",
								}}
							>
								{cdr.userData}
							</Text>
						</Card>
					)}
				</div>
			</div>
		</div>
	);
};

export default CdrDetailPage;
