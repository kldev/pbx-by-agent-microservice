import {
	Card,
	CardHeader,
	makeStyles,
	mergeClasses,
	Table,
	TableBody,
	TableCell,
	TableHeader,
	TableHeaderCell,
	TableRow,
	Text,
	Tooltip,
	tokens,
} from "@fluentui/react-components";
import {
	ClockRegular,
	MoneyRegular,
	PersonRegular,
} from "@fluentui/react-icons";
import type React from "react";
import type { CallRecordResponse } from "../../../api/cdr/models";
import { CdrTypeBadge } from "./";

const useStyles = makeStyles({
	tableContainer: {
		display: "block",
		"@media (max-width: 960px)": {
			display: "none",
		},
	},
	table: {
		width: "100%",
		backgroundColor: tokens.colorNeutralBackground1,
		tableLayout: "fixed",
	},
	headerRow: {
		backgroundColor: tokens.colorNeutralBackground4,
	},
	headerCell: {
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorNeutralForeground1,
	},
	row: {
		cursor: "pointer",
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1Hover,
		},
	},
	rowEven: {
		cursor: "pointer",
		backgroundColor: tokens.colorNeutralBackground2,
		":hover": {
			backgroundColor: tokens.colorNeutralBackground2Hover,
		},
	},
	callerIdCell: {
		width: "140px",
		fontFamily: "monospace",
	},
	calledNumberCell: {
		width: "140px",
		fontFamily: "monospace",
	},
	startTimeCell: {
		width: "160px",
	},
	durationCell: {
		width: "100px",
		textAlign: "right",
	},
	costCell: {
		width: "120px",
		textAlign: "right",
	},
	statusCell: {
		width: "120px",
	},
	customerCell: {
		width: "150px",
	},
	truncatedCell: {
		display: "block",
		overflow: "hidden",
		textOverflow: "ellipsis",
		whiteSpace: "nowrap",
	},
	cardsContainer: {
		display: "none",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
		"@media (max-width: 960px)": {
			display: "flex",
		},
	},
	card: {
		cursor: "pointer",
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1Hover,
		},
	},
	cardContent: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalS,
		padding: tokens.spacingVerticalS,
	},
	cardRow: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
		color: tokens.colorNeutralForeground2,
		fontSize: tokens.fontSizeBase200,
	},
	cardRowIcon: {
		flexShrink: 0,
		color: tokens.colorNeutralForeground3,
	},
	cardRowText: {
		overflow: "hidden",
		textOverflow: "ellipsis",
		whiteSpace: "nowrap",
	},
});

export interface CdrTableProps {
	items: CallRecordResponse[];
	onRowClick?: (item: CallRecordResponse) => void;
}

const TruncatedCell: React.FC<{ value?: string | null }> = ({ value }) => {
	const styles = useStyles();
	const displayValue = value || "-";

	return (
		<Tooltip content={displayValue} relationship="description">
			<span className={styles.truncatedCell}>{displayValue}</span>
		</Tooltip>
	);
};

function formatDuration(seconds?: number): string {
	if (seconds === undefined || seconds === null) return "-";
	const mins = Math.floor(seconds / 60);
	const secs = seconds % 60;
	return `${mins}:${secs.toString().padStart(2, "0")}`;
}

function formatDateTime(dateStr?: string): string {
	if (!dateStr) return "-";
	const date = new Date(dateStr);
	return date.toLocaleString("pl-PL", {
		day: "2-digit",
		month: "2-digit",
		year: "numeric",
		hour: "2-digit",
		minute: "2-digit",
	});
}

function formatCost(cost?: number, currency?: string | null): string {
	if (cost === undefined || cost === null) return "-";
	const formattedCost = cost.toFixed(4);
	return currency ? `${formattedCost} ${currency}` : formattedCost;
}

const CdrTable: React.FC<CdrTableProps> = ({ items, onRowClick }) => {
	const styles = useStyles();

	const TableView = (
		<div className={styles.tableContainer}>
			<Table aria-label="Lista połączeń" className={styles.table}>
				<TableHeader>
					<TableRow className={styles.headerRow}>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.callerIdCell)}
						>
							Z numeru
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(
								styles.headerCell,
								styles.calledNumberCell,
							)}
						>
							Na numer
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.startTimeCell)}
						>
							Data i czas
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.durationCell)}
						>
							Czas trwania
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.costCell)}
						>
							Koszt
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.statusCell)}
						>
							Status
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.customerCell)}
						>
							Klient
						</TableHeaderCell>
					</TableRow>
				</TableHeader>
				<TableBody>
					{items.map((item, index) => (
						<TableRow
							key={item.gid}
							onClick={() => onRowClick?.(item)}
							className={index % 2 === 1 ? styles.rowEven : styles.row}
						>
							<TableCell className={styles.callerIdCell}>
								<TruncatedCell value={item.callerId} />
							</TableCell>
							<TableCell className={styles.calledNumberCell}>
								<TruncatedCell value={item.calledNumber} />
							</TableCell>
							<TableCell className={styles.startTimeCell}>
								<TruncatedCell value={formatDateTime(item.startTime)} />
							</TableCell>
							<TableCell className={styles.durationCell}>
								{formatDuration(item.duration)}
							</TableCell>
							<TableCell className={styles.costCell}>
								{formatCost(item.totalCost, item.currencyCode)}
							</TableCell>
							<TableCell className={styles.statusCell}>
								<CdrTypeBadge
									statusCode={item.callStatusCode ?? ""}
									statusDesc={item.callStatusName ?? ""}
								/>
							</TableCell>
							<TableCell className={styles.customerCell}>
								<TruncatedCell value={item.customerName} />
							</TableCell>
						</TableRow>
					))}
				</TableBody>
			</Table>
		</div>
	);

	const CardView = (
		<div className={styles.cardsContainer}>
			{items.map((item) => (
				<Card
					key={item.gid}
					className={styles.card}
					onClick={() => onRowClick?.(item)}
				>
					<CardHeader
						header={
							<Text weight="semibold">
								{item.callerId || "Nieznany"} →{" "}
								{item.calledNumber || "Nieznany"}
							</Text>
						}
						description={
							<div
								style={{
									display: "flex",
									alignItems: "center",
									gap: "8px",
									marginTop: "4px",
								}}
							>
								<CdrTypeBadge
									statusCode={item.callStatusCode ?? ""}
									statusDesc={item.callStatusName ?? ""}
								/>
							</div>
						}
					/>
					<div className={styles.cardContent}>
						<div className={styles.cardRow}>
							<ClockRegular className={styles.cardRowIcon} />
							<span className={styles.cardRowText}>
								{formatDateTime(item.startTime)} (
								{formatDuration(item.duration)})
							</span>
						</div>
						{item.totalCost !== undefined && (
							<div className={styles.cardRow}>
								<MoneyRegular className={styles.cardRowIcon} />
								<span className={styles.cardRowText}>
									{formatCost(item.totalCost, item.currencyCode)}
								</span>
							</div>
						)}
						{item.customerName && (
							<div className={styles.cardRow}>
								<PersonRegular className={styles.cardRowIcon} />
								<span className={styles.cardRowText}>{item.customerName}</span>
							</div>
						)}
					</div>
				</Card>
			))}
		</div>
	);

	return (
		<>
			{TableView}
			{CardView}
		</>
	);
};

export default CdrTable;
