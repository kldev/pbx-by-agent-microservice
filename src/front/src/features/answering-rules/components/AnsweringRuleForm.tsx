import {
	Button,
	Checkbox,
	Dropdown,
	Field,
	Input,
	Option,
	Spinner,
	SpinButton,
	Textarea,
	makeStyles,
	tokens,
} from "@fluentui/react-components";
import { DismissRegular, SaveRegular } from "@fluentui/react-icons";
import type React from "react";
import { useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import {
	AnsweringRuleAction,
	type TimeSlotDto,
} from "../../../api/answerrule/models";
import SipAccountPicker from "./SipAccountPicker";
import TimeSlotsEditor from "./TimeSlotsEditor";

const ACTION_TYPE_LABELS: Record<AnsweringRuleAction, string> = {
	Voicemail: "Poczta głosowa",
	Redirect: "Przekierowanie",
	RedirectToGroup: "Przekierowanie do grupy",
	DisconnectWithVoicemessage: "Rozłączenie z komunikatem",
};

const ACTION_TYPES = Object.entries(ACTION_TYPE_LABELS) as [
	AnsweringRuleAction,
	string,
][];

const useStyles = makeStyles({
	form: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalL,
		maxWidth: "700px",
	},
	row: {
		display: "grid",
		gridTemplateColumns: "1fr 1fr",
		gap: tokens.spacingHorizontalL,
		"@media (max-width: 600px)": {
			gridTemplateColumns: "1fr",
		},
	},
	section: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
		padding: tokens.spacingVerticalM,
		border: `1px solid ${tokens.colorNeutralStroke2}`,
		borderRadius: tokens.borderRadiusMedium,
	},
	sectionTitle: {
		fontWeight: tokens.fontWeightSemibold,
		fontSize: tokens.fontSizeBase400,
	},
	actions: {
		display: "flex",
		gap: tokens.spacingHorizontalM,
		marginTop: tokens.spacingVerticalL,
		paddingTop: tokens.spacingVerticalL,
		borderTop: `1px solid ${tokens.colorNeutralStroke1}`,
	},
});

export interface AnsweringRuleFormData {
	sipAccountGid: string;
	name: string;
	description: string;
	priority: number;
	isEnabled: boolean;
	actionType: AnsweringRuleAction;
	actionTarget: string;
	voicemailBoxGid: string;
	voiceMessageGid: string;
	sendEmailNotification: boolean;
	notificationEmail: string;
	timeSlots: TimeSlotDto[];
}

export interface AnsweringRuleFormProps {
	initialData?: Partial<AnsweringRuleFormData>;
	onSubmit: (data: AnsweringRuleFormData) => void;
	onCancel: () => void;
	isLoading?: boolean;
	submitLabel?: string;
	isEditMode?: boolean;
}

const AnsweringRuleForm: React.FC<AnsweringRuleFormProps> = ({
	initialData,
	onSubmit,
	onCancel,
	isLoading = false,
	submitLabel = "Zapisz",
	isEditMode = false,
}) => {
	const styles = useStyles();

	const {
		control,
		handleSubmit,
		watch,
		reset,
		setValue,
		formState: { errors },
	} = useForm<AnsweringRuleFormData>({
		defaultValues: {
			sipAccountGid: "",
			name: "",
			description: "",
			priority: 100,
			isEnabled: true,
			actionType: AnsweringRuleAction.Voicemail,
			actionTarget: "",
			voicemailBoxGid: "",
			voiceMessageGid: "",
			sendEmailNotification: false,
			notificationEmail: "",
			timeSlots: [],
			...initialData,
		},
	});

	const actionType = watch("actionType");
	const sendEmailNotification = watch("sendEmailNotification");

	useEffect(() => {
		if (initialData) {
			reset({
				sipAccountGid: "",
				name: "",
				description: "",
				priority: 100,
				isEnabled: true,
				actionType: AnsweringRuleAction.Voicemail,
				actionTarget: "",
				voicemailBoxGid: "",
				voiceMessageGid: "",
				sendEmailNotification: false,
				notificationEmail: "",
				timeSlots: [],
				...initialData,
			});
		}
	}, [initialData, reset]);

	const showActionTarget =
		actionType === AnsweringRuleAction.Redirect ||
		actionType === AnsweringRuleAction.RedirectToGroup;
	const showVoicemailBox = actionType === AnsweringRuleAction.Voicemail;
	const showVoiceMessage =
		actionType === AnsweringRuleAction.DisconnectWithVoicemessage;

	return (
		<form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
			{/* Konto SIP */}
			<Field
				label="Konto SIP *"
				validationState={errors.sipAccountGid ? "error" : "none"}
				validationMessage={errors.sipAccountGid?.message}
			>
				<Controller
					name="sipAccountGid"
					control={control}
					rules={{ required: "Konto SIP jest wymagane" }}
					render={({ field }) => (
						<SipAccountPicker
							value={field.value}
							onChange={field.onChange}
							disabled={isEditMode}
						/>
					)}
				/>
			</Field>

			{/* Nazwa i priorytet */}
			<div className={styles.row}>
				<Field
					label="Nazwa *"
					validationState={errors.name ? "error" : "none"}
					validationMessage={errors.name?.message}
				>
					<Controller
						name="name"
						control={control}
						rules={{
							required: "Nazwa jest wymagana",
							maxLength: { value: 100, message: "Maksymalnie 100 znaków" },
						}}
						render={({ field }) => (
							<Input {...field} placeholder="np. Poza godzinami pracy" />
						)}
					/>
				</Field>

				<Field
					label="Priorytet"
					validationState={errors.priority ? "error" : "none"}
					validationMessage={errors.priority?.message}
				>
					<Controller
						name="priority"
						control={control}
						rules={{
							required: "Priorytet jest wymagany",
							min: { value: 0, message: "Minimum 0" },
						}}
						render={({ field }) => (
							<SpinButton
								value={field.value}
								onChange={(_, data) => {
									if (data.value !== undefined && data.value !== null) {
										field.onChange(data.value);
									}
								}}
								min={0}
								max={9999}
								step={10}
							/>
						)}
					/>
				</Field>
			</div>

			{/* Opis */}
			<Field
				label="Opis"
				validationState={errors.description ? "error" : "none"}
				validationMessage={errors.description?.message}
			>
				<Controller
					name="description"
					control={control}
					rules={{
						maxLength: { value: 500, message: "Maksymalnie 500 znaków" },
					}}
					render={({ field }) => (
						<Textarea {...field} placeholder="Opis reguły..." rows={2} />
					)}
				/>
			</Field>

			{/* Typ akcji */}
			<div className={styles.section}>
				<span className={styles.sectionTitle}>Akcja</span>

				<Field
					label="Typ akcji *"
					validationState={errors.actionType ? "error" : "none"}
					validationMessage={errors.actionType?.message}
				>
					<Controller
						name="actionType"
						control={control}
						rules={{ required: "Typ akcji jest wymagany" }}
						render={({ field }) => (
							<Dropdown
								value={ACTION_TYPE_LABELS[field.value] ?? ""}
								selectedOptions={[field.value]}
								onOptionSelect={(_, data) => {
									if (data.optionValue) {
										field.onChange(data.optionValue as AnsweringRuleAction);
									}
								}}
							>
								{ACTION_TYPES.map(([value, label]) => (
									<Option key={value} value={value} text={label}>
										{label}
									</Option>
								))}
							</Dropdown>
						)}
					/>
				</Field>

				{showActionTarget && (
					<Field
						label={
							actionType === AnsweringRuleAction.Redirect
								? "Numer docelowy *"
								: "GID grupy *"
						}
						validationState={errors.actionTarget ? "error" : "none"}
						validationMessage={errors.actionTarget?.message}
					>
						<Controller
							name="actionTarget"
							control={control}
							rules={{
								required: "Cel akcji jest wymagany",
							}}
							render={({ field }) => (
								<Input
									{...field}
									placeholder={
										actionType === AnsweringRuleAction.Redirect
											? "+48123456789"
											: "GID grupy"
									}
								/>
							)}
						/>
					</Field>
				)}

				{showVoicemailBox && (
					<Field
						label="GID skrzynki voicemail *"
						validationState={errors.voicemailBoxGid ? "error" : "none"}
						validationMessage={errors.voicemailBoxGid?.message}
					>
						<Controller
							name="voicemailBoxGid"
							control={control}
							rules={{
								required: "Skrzynka voicemail jest wymagana",
							}}
							render={({ field }) => (
								<Input {...field} placeholder="GID skrzynki voicemail" />
							)}
						/>
					</Field>
				)}

				{showVoiceMessage && (
					<Field
						label="GID komunikatu głosowego *"
						validationState={errors.voiceMessageGid ? "error" : "none"}
						validationMessage={errors.voiceMessageGid?.message}
					>
						<Controller
							name="voiceMessageGid"
							control={control}
							rules={{
								required: "Komunikat głosowy jest wymagany",
							}}
							render={({ field }) => (
								<Input {...field} placeholder="GID komunikatu głosowego" />
							)}
						/>
					</Field>
				)}
			</div>

			{/* Notyfikacje */}
			<div className={styles.section}>
				<span className={styles.sectionTitle}>Notyfikacje</span>

				<Controller
					name="sendEmailNotification"
					control={control}
					render={({ field }) => (
						<Checkbox
							checked={field.value}
							onChange={(_, data) => field.onChange(data.checked)}
							label="Wysyłaj powiadomienie email"
						/>
					)}
				/>

				{sendEmailNotification && (
					<Field
						label="Email do powiadomień"
						validationState={errors.notificationEmail ? "error" : "none"}
						validationMessage={errors.notificationEmail?.message}
					>
						<Controller
							name="notificationEmail"
							control={control}
							rules={{
								pattern: {
									value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
									message: "Nieprawidłowy format email",
								},
							}}
							render={({ field }) => (
								<Input {...field} type="email" placeholder="user@example.com" />
							)}
						/>
					</Field>
				)}
			</div>

			{/* Status (tylko edycja) */}
			{isEditMode && (
				<Controller
					name="isEnabled"
					control={control}
					render={({ field }) => (
						<Checkbox
							checked={field.value}
							onChange={(_, data) => field.onChange(data.checked)}
							label="Reguła aktywna"
						/>
					)}
				/>
			)}

			{/* Przedziały czasowe */}
			<div className={styles.section}>
				<span className={styles.sectionTitle}>Przedziały czasowe *</span>
				<Controller
					name="timeSlots"
					control={control}
					rules={{
						validate: (val) =>
							(val && val.length > 0) ||
							"Reguła musi mieć co najmniej jeden przedział czasowy",
					}}
					render={({ field }) => (
						<TimeSlotsEditor
							value={field.value ?? []}
							onChange={(slots) =>
								setValue("timeSlots", slots, { shouldValidate: true })
							}
							error={errors.timeSlots?.message}
						/>
					)}
				/>
			</div>

			{/* Przyciski */}
			<div className={styles.actions}>
				<Button
					appearance="primary"
					type="submit"
					icon={isLoading ? <Spinner size="tiny" /> : <SaveRegular />}
					disabled={isLoading}
				>
					{submitLabel}
				</Button>
				<Button
					appearance="secondary"
					icon={<DismissRegular />}
					onClick={onCancel}
					disabled={isLoading}
				>
					Anuluj
				</Button>
			</div>
		</form>
	);
};

export default AnsweringRuleForm;
