import {
	Button,
	Dialog,
	DialogActions,
	DialogBody,
	DialogContent,
	DialogSurface,
	DialogTitle,
	Dropdown,
	Field,
	makeStyles,
	Option,
	Text,
	Textarea,
	tokens,
} from "@fluentui/react-components";
import { DeleteRegular, Dismiss24Regular } from "@fluentui/react-icons";
import type React from "react";
import { useState } from "react";
import {
	useDeleteDayEntry,
	useSaveDayEntry,
} from "../../../../api/rcp/endpoints/rcp-employee/rcp-employee";
import type { DayEntryResponse } from "../../../../api/rcp/models";
import {
	calculateEndTime,
	getHourOptions,
	getMinuteOptions,
	getPolishDayNameFull,
	getStartTimeOptions,
} from "../../utils";
import { DialogHint } from "../shared";

const useStyles = makeStyles({
	content: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
	},
	row: {
		display: "grid",
		gridTemplateColumns: "1fr 1fr",
		gap: tokens.spacingHorizontalM,
	},
	endTime: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
		padding: tokens.spacingVerticalS,
		backgroundColor: tokens.colorNeutralBackground3,
		borderRadius: tokens.borderRadiusMedium,
	},
	endTimeLabel: {
		color: tokens.colorNeutralForeground3,
	},
	endTimeValue: {
		fontWeight: tokens.fontWeightSemibold,
	},
	deleteButton: {
		color: tokens.colorPaletteRedForeground1,
	},
	closeButton: {
		minWidth: "auto",
		padding: tokens.spacingHorizontalXS,
	},
});

interface TimeEntryDialogProps {
	year: number;
	month: number;
	date: string;
	existingEntry?: DayEntryResponse;
	onClose: () => void;
	onSuccess: () => void;
}

const TimeEntryDialog: React.FC<TimeEntryDialogProps> = ({
	year,
	month,
	date,
	existingEntry,
	onClose,
	onSuccess,
}) => {
	const styles = useStyles();

	const saveMutation = useSaveDayEntry();
	const deleteMutation = useDeleteDayEntry();

	const [startTime, setStartTime] = useState(
		existingEntry?.startTime ?? "08:00",
	);
	const [hours, setHours] = useState(existingEntry?.hours ?? 8);
	const [minutes, setMinutes] = useState(existingEntry?.minutes ?? 0);
	const [notes, setNotes] = useState(existingEntry?.notes ?? "");

	const endTime = calculateEndTime(startTime, hours, minutes);

	const dateObj = new Date(date);
	const dayName = getPolishDayNameFull(dateObj.getDay());
	const formattedDate = dateObj.toLocaleDateString("pl-PL", {
		day: "numeric",
		month: "long",
		year: "numeric",
	});

	const handleSave = async () => {
		try {
			await saveMutation.mutateAsync({
				data: {
					year,
					month,
					workDate: date,
					startTime,
					hours,
					minutes,
					notes: notes || null,
				},
			});
			onSuccess();
		} catch (error) {
			console.error("Failed to save entry:", error);
		}
	};

	const handleDelete = async () => {
		try {
			await deleteMutation.mutateAsync({
				year,
				month,
				date,
			});
			onSuccess();
		} catch (error) {
			console.error("Failed to delete entry:", error);
		}
	};

	const isLoading = saveMutation.isPending || deleteMutation.isPending;

	return (
		<Dialog open onOpenChange={(_, data) => !data.open && onClose()}>
			<DialogSurface>
				<DialogBody>
					<DialogTitle
						action={
							<Button
								appearance="subtle"
								icon={<Dismiss24Regular />}
								onClick={onClose}
								className={styles.closeButton}
								disabled={isLoading}
							/>
						}
					>
						{dayName}, {formattedDate}
					</DialogTitle>
					<DialogContent className={styles.content}>
						<DialogHint>
							Wpisz godzinę rozpoczęcia pracy i czas pracy w godzinach i
							minutach. Godzina zakończenia zostanie obliczona automatycznie. W
							notatkach możesz wpisać np. "praca zdalna", "spotkanie u klienta".
						</DialogHint>
						<Field label="Godzina rozpoczęcia">
							<Dropdown
								value={startTime}
								selectedOptions={[startTime]}
								onOptionSelect={(_, data) =>
									setStartTime(data.optionValue ?? "08:00")
								}
							>
								{getStartTimeOptions().map((opt) => (
									<Option key={opt.value} value={opt.value}>
										{opt.label}
									</Option>
								))}
							</Dropdown>
						</Field>

						<div className={styles.row}>
							<Field label="Godziny">
								<Dropdown
									value={hours.toString()}
									selectedOptions={[hours.toString()]}
									onOptionSelect={(_, data) =>
										setHours(Number(data.optionValue) || 0)
									}
								>
									{getHourOptions().map((opt) => (
										<Option key={opt.value} value={opt.value.toString()}>
											{opt.label}
										</Option>
									))}
								</Dropdown>
							</Field>
							<Field label="Minuty">
								<Dropdown
									value={minutes.toString()}
									selectedOptions={[minutes.toString()]}
									onOptionSelect={(_, data) =>
										setMinutes(Number(data.optionValue) || 0)
									}
								>
									{getMinuteOptions().map((opt) => (
										<Option key={opt.value} value={opt.value.toString()}>
											{opt.label}
										</Option>
									))}
								</Dropdown>
							</Field>
						</div>

						<div className={styles.endTime}>
							<Text className={styles.endTimeLabel}>Godzina zakończenia:</Text>
							<Text className={styles.endTimeValue}>{endTime}</Text>
						</div>

						<Field label="Notatki (opcjonalnie)">
							<Textarea
								value={notes}
								onChange={(_, data) => setNotes(data.value)}
								placeholder="Dodaj notatki do tego wpisu..."
								rows={3}
							/>
						</Field>
					</DialogContent>
					<DialogActions>
						{existingEntry && (
							<Button
								appearance="subtle"
								icon={<DeleteRegular />}
								onClick={handleDelete}
								disabled={isLoading}
								className={styles.deleteButton}
							>
								Usuń
							</Button>
						)}
						<Button
							appearance="primary"
							onClick={handleSave}
							disabled={isLoading || (hours === 0 && minutes === 0)}
						>
							{isLoading ? "Zapisywanie..." : "Zapisz"}
						</Button>
					</DialogActions>
				</DialogBody>
			</DialogSurface>
		</Dialog>
	);
};

export default TimeEntryDialog;
