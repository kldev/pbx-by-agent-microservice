import {
	Button,
	Checkbox,
	Dropdown,
	Field,
	Input,
	makeStyles,
	Option,
	type OptionOnSelectData,
	type SelectionEvents,
	Spinner,
	Text,
	tokens,
} from "@fluentui/react-components";
import { DismissRegular, SaveRegular } from "@fluentui/react-icons";
import type React from "react";
import { useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { Department } from "../../../api/identity/models";
import RolePicker from "./RolePicker";

const useStyles = makeStyles({
	form: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalL,
		maxWidth: "800px",
	},
	row: {
		display: "grid",
		gridTemplateColumns: "1fr 1fr",
		gap: tokens.spacingHorizontalL,
		"@media (max-width: 600px)": {
			gridTemplateColumns: "1fr",
		},
	},
	fullWidth: {
		gridColumn: "1 / -1",
	},
	rolesSection: {
		marginTop: tokens.spacingVerticalM,
	},
	rolesSectionTitle: {
		fontWeight: tokens.fontWeightSemibold,
		marginBottom: tokens.spacingVerticalS,
	},
	actions: {
		display: "flex",
		gap: tokens.spacingHorizontalM,
		marginTop: tokens.spacingVerticalL,
		paddingTop: tokens.spacingVerticalL,
		borderTop: `1px solid ${tokens.colorNeutralStroke1}`,
	},
});

const departmentOptions = [
	{ value: Department.Sales, label: "Sprzedaż" },
	{ value: Department.Operations, label: "Operacje" },
	{ value: Department.Finance, label: "Finanse" },
	{ value: Department.Developers, label: "IT" },
];

export interface SystemUserFormData {
	firstName: string;
	lastName: string;
	email: string;
	password: string;
	department: (typeof Department)[keyof typeof Department];
	roles: string[];
	isActive: boolean;
}

export interface SystemUserFormProps {
	initialData?: Partial<SystemUserFormData>;
	onSubmit: (data: SystemUserFormData) => void;
	onCancel: () => void;
	isLoading?: boolean;
	submitLabel?: string;
	isEditMode?: boolean;
}

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const SystemUserForm: React.FC<SystemUserFormProps> = ({
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
		reset,
		watch,
		setValue,
		formState: { errors },
	} = useForm<SystemUserFormData>({
		defaultValues: {
			firstName: "",
			lastName: "",
			email: "",
			password: "",
			department: Department.Sales,
			roles: ["ResponsiblePerson"],
			isActive: true,
			...initialData,
		},
	});

	const selectedDepartment = watch("department");
	const selectedRoles = watch("roles");

	useEffect(() => {
		if (initialData) {
			reset({
				firstName: "",
				lastName: "",
				email: "",
				password: "",
				department: Department.Sales,
				roles: ["ResponsiblePerson"],
				isActive: true,
				...initialData,
			});
		}
	}, [initialData, reset]);

	const handleDepartmentChange = (
		_ev: SelectionEvents,
		data: OptionOnSelectData,
	) => {
		if (data.optionValue) {
			setValue(
				"department",
				data.optionValue as (typeof Department)[keyof typeof Department],
			);
		}
	};

	const handleRolesChange = (roles: string[]) => {
		setValue("roles", roles);
	};

	const selectedDepartmentLabel = departmentOptions.find(
		(opt) => opt.value === selectedDepartment,
	)?.label;

	return (
		<form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
			<div className={styles.row}>
				<Field
					label="Imię *"
					validationState={errors.firstName ? "error" : "none"}
					validationMessage={errors.firstName?.message}
				>
					<Controller
						name="firstName"
						control={control}
						rules={{
							required: "Imię jest wymagane",
							maxLength: {
								value: 100,
								message: "Maksymalnie 100 znaków",
							},
						}}
						render={({ field }) => (
							<Input {...field} placeholder="Wprowadź imię" />
						)}
					/>
				</Field>

				<Field
					label="Nazwisko *"
					validationState={errors.lastName ? "error" : "none"}
					validationMessage={errors.lastName?.message}
				>
					<Controller
						name="lastName"
						control={control}
						rules={{
							required: "Nazwisko jest wymagane",
							maxLength: {
								value: 100,
								message: "Maksymalnie 100 znaków",
							},
						}}
						render={({ field }) => (
							<Input {...field} placeholder="Wprowadź nazwisko" />
						)}
					/>
				</Field>
			</div>

			<div className={styles.row}>
				<Field
					label="Email *"
					validationState={errors.email ? "error" : "none"}
					validationMessage={errors.email?.message}
				>
					<Controller
						name="email"
						control={control}
						rules={{
							required: "Email jest wymagany",
							pattern: {
								value: emailRegex,
								message: "Nieprawidłowy format email",
							},
						}}
						render={({ field }) => (
							<Input {...field} type="email" placeholder="email@firma.pl" />
						)}
					/>
				</Field>

				{!isEditMode && (
					<Field
						label="Hasło *"
						validationState={errors.password ? "error" : "none"}
						validationMessage={errors.password?.message}
					>
						<Controller
							name="password"
							control={control}
							rules={{
								required: "Hasło jest wymagane",
								minLength: {
									value: 6,
									message: "Hasło musi mieć co najmniej 6 znaków",
								},
							}}
							render={({ field }) => (
								<Input
									{...field}
									type="password"
									placeholder="Wprowadź hasło"
								/>
							)}
						/>
					</Field>
				)}

				{isEditMode && (
					<Field
						label="Dział *"
						validationState={errors.department ? "error" : "none"}
						validationMessage={errors.department?.message}
					>
						<Dropdown
							value={selectedDepartmentLabel}
							selectedOptions={[selectedDepartment]}
							onOptionSelect={handleDepartmentChange}
							placeholder="Wybierz dział"
						>
							{departmentOptions.map((option) => (
								<Option key={option.value} value={option.value}>
									{option.label}
								</Option>
							))}
						</Dropdown>
					</Field>
				)}
			</div>

			{!isEditMode && (
				<div className={styles.row}>
					<Field
						label="Dział *"
						validationState={errors.department ? "error" : "none"}
						validationMessage={errors.department?.message}
					>
						<Dropdown
							value={selectedDepartmentLabel}
							selectedOptions={[selectedDepartment]}
							onOptionSelect={handleDepartmentChange}
							placeholder="Wybierz dział"
						>
							{departmentOptions.map((option) => (
								<Option key={option.value} value={option.value}>
									{option.label}
								</Option>
							))}
						</Dropdown>
					</Field>
					<div />
				</div>
			)}

			{isEditMode && (
				<Controller
					name="isActive"
					control={control}
					render={({ field }) => (
						<Checkbox
							checked={field.value}
							onChange={(_, data) => field.onChange(data.checked)}
							label="Użytkownik aktywny"
						/>
					)}
				/>
			)}

			<div className={styles.rolesSection}>
				<Text className={styles.rolesSectionTitle}>
					Uprawnienia użytkownika
				</Text>
				<RolePicker
					selectedRoles={selectedRoles}
					onChange={handleRolesChange}
					disabled={isLoading}
				/>
			</div>

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

export default SystemUserForm;
