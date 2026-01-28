import {
	makeStyles,
	mergeClasses,
	ProgressBar,
	Table,
	TableBody,
	TableCell,
	TableHeader,
	TableHeaderCell,
	TableRow,
	Text,
	tokens,
} from "@fluentui/react-components";
import type React from "react";
import type { MonitorEntrySummary } from "../../../../api/rcp/models";
import RcpStatusBadge from "../RcpStatusBadge";

const useStyles = makeStyles({
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
	employeeCell: {
		fontWeight: tokens.fontWeightSemibold,
		width: "30%",
	},
	statusCell: {
		width: "20%",
	},
	hoursCell: {
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorBrandForeground1,
		width: "12%",
	},
	progressCell: {
		width: "25%",
	},
	progressContainer: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXXS,
	},
	progressText: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
	dateCell: {
		width: "13%",
	},
	emptyState: {
		textAlign: "center",
		padding: tokens.spacingVerticalXXL,
		color: tokens.colorNeutralForeground3,
	},
});

interface MonitoringTableProps {
	entries: MonitorEntrySummary[];
	workingDaysInMonth: number;
	onEntryClick: (gid: string) => void;
}

const MonitoringTable: React.FC<MonitoringTableProps> = ({
	entries,
	workingDaysInMonth,
	onEntryClick,
}) => {
	const styles = useStyles();

	if (entries.length === 0) {
		return (
			<div className={styles.emptyState}>
				<Text>Brak podwładnych do wyświetlenia</Text>
			</div>
		);
	}

	return (
		<Table className={styles.table} aria-label="Monitoring podwładnych">
			<TableHeader>
				<TableRow className={styles.headerRow}>
					<TableHeaderCell
						className={mergeClasses(styles.headerCell, styles.employeeCell)}
					>
						Pracownik
					</TableHeaderCell>
					<TableHeaderCell
						className={mergeClasses(styles.headerCell, styles.statusCell)}
					>
						Status
					</TableHeaderCell>
					<TableHeaderCell
						className={mergeClasses(styles.headerCell, styles.hoursCell)}
					>
						Godziny
					</TableHeaderCell>
					<TableHeaderCell
						className={mergeClasses(styles.headerCell, styles.progressCell)}
					>
						Postęp
					</TableHeaderCell>
					<TableHeaderCell
						className={mergeClasses(styles.headerCell, styles.dateCell)}
					>
						Ostatni wpis
					</TableHeaderCell>
				</TableRow>
			</TableHeader>
			<TableBody>
				{entries.map((entry, index) => {
					const filledDays = entry.filledDays ?? 0;
					const progress =
						workingDaysInMonth > 0 ? filledDays / workingDaysInMonth : 0;

					return (
						<TableRow
							key={entry.gid ?? entry.userId}
							className={index % 2 === 1 ? styles.rowEven : styles.row}
							onClick={() => entry.gid && onEntryClick(entry.gid)}
						>
							<TableCell className={styles.employeeCell}>
								{entry.userFullName ?? "—"}
							</TableCell>
							<TableCell className={styles.statusCell}>
								<RcpStatusBadge status={entry.status} />
							</TableCell>
							<TableCell className={styles.hoursCell}>
								{entry.totalHours?.toFixed(1) ?? "0.0"}h
							</TableCell>
							<TableCell className={styles.progressCell}>
								<div className={styles.progressContainer}>
									<ProgressBar
										value={progress}
										max={1}
										color={progress >= 1 ? "success" : "brand"}
									/>
									<Text className={styles.progressText}>
										{filledDays} / {workingDaysInMonth} dni
									</Text>
								</div>
							</TableCell>
							<TableCell className={styles.dateCell}>
								{entry.lastEntryDate
									? new Date(entry.lastEntryDate).toLocaleDateString("pl-PL")
									: "—"}
							</TableCell>
						</TableRow>
					);
				})}
			</TableBody>
		</Table>
	);
};

export default MonitoringTable;
