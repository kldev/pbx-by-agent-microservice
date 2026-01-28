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
	Table,
	TableBody,
	TableCell,
	TableHeader,
	TableHeaderCell,
	TableRow,
	Text,
	Tooltip,
	makeStyles,
	mergeClasses,
	tokens,
} from "@fluentui/react-components";
import {
	DeleteRegular,
	EditRegular,
	EyeRegular,
	MoreVerticalRegular,
	ToggleLeftRegular,
} from "@fluentui/react-icons";
import type React from "react";
import type { AnsweringRuleResponse } from "../../../api/answerrule/models";
import ActionTypeBadge from "./ActionTypeBadge";

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
	actionsCell: { width: "48px" },
	nameCell: { width: "200px" },
	actionTypeCell: { width: "200px" },
	targetCell: { width: "150px" },
	priorityCell: { width: "80px" },
	slotsCell: { width: "80px" },
	statusCell: { width: "100px" },
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
});

export interface AnsweringRuleTableProps {
	items: AnsweringRuleResponse[];
	onRowClick?: (item: AnsweringRuleResponse) => void;
	onEdit?: (item: AnsweringRuleResponse) => void;
	onDelete?: (item: AnsweringRuleResponse) => void;
	onToggle?: (item: AnsweringRuleResponse) => void;
	onShowDetails?: (item: AnsweringRuleResponse) => void;
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

const StatusBadge: React.FC<{ isEnabled?: boolean }> = ({ isEnabled }) => (
	<Badge
		appearance="tint"
		color={isEnabled ? "success" : "danger"}
		size="small"
	>
		{isEnabled ? "Aktywna" : "Nieaktywna"}
	</Badge>
);

const AnsweringRuleTable: React.FC<AnsweringRuleTableProps> = ({
	items,
	onRowClick,
	onEdit,
	onDelete,
	onToggle,
	onShowDetails,
}) => {
	const styles = useStyles();

	const handleActionClick = (e: React.MouseEvent) => {
		e.stopPropagation();
	};

	const handleShowDetails = (item: AnsweringRuleResponse) => {
		onShowDetails ? onShowDetails(item) : onRowClick?.(item);
	};

	const TableView = (
		<div className={styles.tableContainer}>
			<Table aria-label="Lista reguł odpowiadania" className={styles.table}>
				<TableHeader>
					<TableRow className={styles.headerRow}>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.actionsCell)}
						/>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.nameCell)}
						>
							Nazwa
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.actionTypeCell)}
						>
							Typ akcji
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.targetCell)}
						>
							Cel
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.priorityCell)}
						>
							Priorytet
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.slotsCell)}
						>
							Sloty
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
												onClick={() => onToggle?.(item)}
											>
												{item.isEnabled ? "Wyłącz" : "Włącz"}
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
							<TableCell className={styles.nameCell}>
								<TruncatedCell value={item.name} />
							</TableCell>
							<TableCell className={styles.actionTypeCell}>
								{item.actionType && (
									<ActionTypeBadge actionType={item.actionType} />
								)}
							</TableCell>
							<TableCell className={styles.targetCell}>
								<TruncatedCell value={item.actionTarget} />
							</TableCell>
							<TableCell className={styles.priorityCell}>
								<Badge appearance="outline" size="small">
									{item.priority}
								</Badge>
							</TableCell>
							<TableCell className={styles.slotsCell}>
								{item.timeSlotsCount ?? 0}
							</TableCell>
							<TableCell className={styles.statusCell}>
								<StatusBadge isEnabled={item.isEnabled} />
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
							<Text weight="semibold">{item.name || "Brak nazwy"}</Text>
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
								<StatusBadge isEnabled={item.isEnabled} />
								{item.actionType && (
									<ActionTypeBadge actionType={item.actionType} />
								)}
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
												onClick={() => onToggle?.(item)}
											>
												{item.isEnabled ? "Wyłącz" : "Włącz"}
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
						<div className={styles.cardRow}>
							<Text>Priorytet: {item.priority}</Text>
							<Text>Sloty: {item.timeSlotsCount ?? 0}</Text>
						</div>
						{item.actionTarget && (
							<div className={styles.cardRow}>
								<Text>Cel: {item.actionTarget}</Text>
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

export default AnsweringRuleTable;
