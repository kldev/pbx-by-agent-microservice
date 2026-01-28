import {
	Badge,
	Button,
	Card,
	CardHeader,
	Menu,
	MenuItem,
	MenuList,
	MenuPopover,
	MenuTrigger,
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
	BuildingRegular,
	DeleteRegular,
	EditRegular,
	EyeRegular,
	MoreVerticalRegular,
	ToggleLeftRegular,
} from "@fluentui/react-icons";
import type React from "react";

import TeamStatusBadge from "./TeamStatusBadge";
import type { TeamResponse } from "../../../api/identity/models";

const useStyles = makeStyles({
	tableContainer: {
		display: "block",
		"@media (max-width: 768px)": {
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
	actionsCell: {
		width: "48px",
	},
	codeCell: {
		width: "100px",
		fontFamily: "monospace",
	},
	nameCell: {
		width: "200px",
	},
	sbuCell: {
		width: "150px",
	},
	descriptionCell: {
		width: "250px",
	},
	statusCell: {
		width: "100px",
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
		"@media (max-width: 768px)": {
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

export interface TeamTableProps {
	items: TeamResponse[];
	onRowClick?: (item: TeamResponse) => void;
	onEdit?: (item: TeamResponse) => void;
	onDelete?: (item: TeamResponse) => void;
	onToggleActive?: (item: TeamResponse) => void;
	onShowDetails?: (item: TeamResponse) => void;
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

const TeamTable: React.FC<TeamTableProps> = ({
	items,
	onRowClick,
	onEdit,
	onDelete,
	onToggleActive,
	onShowDetails,
}) => {
	const styles = useStyles();

	const handleActionClick = (e: React.MouseEvent) => {
		e.stopPropagation();
	};

	const handleShowDetails = (item: TeamResponse) => {
		if (onShowDetails) {
			onShowDetails(item);
		} else if (onRowClick) {
			onRowClick(item);
		}
	};

	const TableView = (
		<div className={styles.tableContainer}>
			<Table aria-label="Lista zespołów" className={styles.table}>
				<TableHeader>
					<TableRow className={styles.headerRow}>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.actionsCell)}
						/>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.codeCell)}
						>
							Kod
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.nameCell)}
						>
							Nazwa
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.sbuCell)}
						>
							SBU
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(
								styles.headerCell,
								styles.descriptionCell,
							)}
						>
							Opis
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.statusCell)}
						>
							Status
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
							<TableCell
								className={styles.actionsCell}
								onClick={handleActionClick}
							>
								<Menu>
									<MenuTrigger disableButtonEnhancement>
										<Button
											appearance="subtle"
											icon={<MoreVerticalRegular />}
											aria-label="Akcje"
										/>
									</MenuTrigger>
									<MenuPopover>
										<MenuList>
											<MenuItem
												icon={<EyeRegular />}
												onClick={() => handleShowDetails(item)}
											>
												Szczegóły
											</MenuItem>
											<MenuItem
												icon={<EditRegular />}
												onClick={() => onEdit?.(item)}
											>
												Edytuj
											</MenuItem>
											<MenuItem
												icon={<ToggleLeftRegular />}
												onClick={() => onToggleActive?.(item)}
											>
												{item.isActive ? "Dezaktywuj" : "Aktywuj"}
											</MenuItem>
											<MenuItem
												icon={<DeleteRegular />}
												onClick={() => onDelete?.(item)}
											>
												Usuń
											</MenuItem>
										</MenuList>
									</MenuPopover>
								</Menu>
							</TableCell>
							<TableCell className={styles.codeCell}>
								<Badge appearance="outline">{item.code || "-"}</Badge>
							</TableCell>
							<TableCell className={styles.nameCell}>
								<TruncatedCell value={item.name} />
							</TableCell>
							<TableCell className={styles.sbuCell}>
								<TruncatedCell value={item.sbuName} />
							</TableCell>
							<TableCell className={styles.descriptionCell}>
								<TruncatedCell value={"-"} />
							</TableCell>
							<TableCell className={styles.statusCell}>
								<TeamStatusBadge isActive={item.isActive ?? false} />
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
						header={<Text weight="semibold">{item.name || "Brak nazwy"}</Text>}
						description={
							<div
								style={{
									display: "flex",
									alignItems: "center",
									gap: "8px",
									marginTop: "4px",
								}}
							>
								<TeamStatusBadge isActive={item.isActive ?? false} />
								<Badge appearance="outline" size="small">
									{item.code}
								</Badge>
							</div>
						}
						action={
							<div onClick={handleActionClick}>
								<Menu>
									<MenuTrigger disableButtonEnhancement>
										<Button
											appearance="subtle"
											icon={<MoreVerticalRegular />}
											aria-label="Akcje"
											size="small"
										/>
									</MenuTrigger>
									<MenuPopover>
										<MenuList>
											<MenuItem
												icon={<EyeRegular />}
												onClick={() => handleShowDetails(item)}
											>
												Szczegóły
											</MenuItem>
											<MenuItem
												icon={<EditRegular />}
												onClick={() => onEdit?.(item)}
											>
												Edytuj
											</MenuItem>
											<MenuItem
												icon={<ToggleLeftRegular />}
												onClick={() => onToggleActive?.(item)}
											>
												{item.isActive ? "Dezaktywuj" : "Aktywuj"}
											</MenuItem>
											<MenuItem
												icon={<DeleteRegular />}
												onClick={() => onDelete?.(item)}
											>
												Usuń
											</MenuItem>
										</MenuList>
									</MenuPopover>
								</Menu>
							</div>
						}
					/>
					<div className={styles.cardContent}>
						{item.sbuName && (
							<div className={styles.cardRow}>
								<BuildingRegular className={styles.cardRowIcon} />
								<span className={styles.cardRowText}>{item.sbuName}</span>
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

export default TeamTable;
