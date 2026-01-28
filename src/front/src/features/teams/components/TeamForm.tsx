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
	Textarea,
	tokens,
} from "@fluentui/react-components";
import { DismissRegular, SaveRegular } from "@fluentui/react-icons";
import type React from "react";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import type { StructureResponse } from "../../../api/identity/models";
import { useGetAllStructure } from "../../../api/identity/endpoints/structure/structure";

const useStyles = makeStyles({
	form: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalL,
		maxWidth: "600px",
	},
	row: {
		display: "grid",
		gridTemplateColumns: "1fr 1fr",
		gap: tokens.spacingHorizontalL,
		"@media (max-width: 600px)": {
			gridTemplateColumns: "1fr",
		},
	},
	actions: {
		display: "flex",
		gap: tokens.spacingHorizontalM,
		marginTop: tokens.spacingVerticalL,
		paddingTop: tokens.spacingVerticalL,
		borderTop: `1px solid ${tokens.colorNeutralStroke1}`,
	},
});

export interface TeamFormData {
	code: string;
	name: string;
	structureId: number;
	description: string;
	isActive: boolean;
}

export interface TeamFormProps {
	initialData?: Partial<TeamFormData>;
	onSubmit: (data: TeamFormData) => void;
	onCancel: () => void;
	isLoading?: boolean;
	submitLabel?: string;
	isEditMode?: boolean;
}

const TeamForm: React.FC<TeamFormProps> = ({
	initialData,
	onSubmit,
	onCancel,
	isLoading = false,
	submitLabel = "Zapisz",
	isEditMode = false,
}) => {
	const styles = useStyles();
	const [sbuOptions, setSbuOptions] = useState<StructureResponse[]>([]);

	const sbuMutation = useGetAllStructure({});

	// biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
	useEffect(() => {
		sbuMutation.mutate({ data: { isActive: true } });
	}, []);

	useEffect(() => {
		if (sbuMutation.data?.status === 200) {
			setSbuOptions(sbuMutation.data.data);
		}
	}, [sbuMutation.data]);

	const {
		control,
		handleSubmit,
		reset,
		watch,
		setValue,
		formState: { errors },
	} = useForm<TeamFormData>({
		defaultValues: {
			code: "",
			name: "",
			structureId: 0,
			description: "",
			isActive: true,
			...initialData,
		},
	});

	const selectedStructureId = watch("structureId");

	useEffect(() => {
		if (initialData) {
			reset({
				code: "",
				name: "",
				structureId: 0,
				description: "",
				isActive: true,
				...initialData,
			});
		}
	}, [initialData, reset]);

	const handleSbuChange = (_ev: SelectionEvents, data: OptionOnSelectData) => {
		if (data.optionValue) {
			setValue("structureId", Number(data.optionValue));
		}
	};

	const selectedSbu = sbuOptions.find((s) => s.id === selectedStructureId);

	return (
		<form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
			<div className={styles.row}>
				<Field
					label="Kod *"
					validationState={errors.code ? "error" : "none"}
					validationMessage={errors.code?.message}
				>
					<Controller
						name="code"
						control={control}
						rules={{
							required: "Kod jest wymagany",
							maxLength: {
								value: 20,
								message: "Maksymalnie 20 znaków",
							},
						}}
						render={({ field }) => (
							<Input
								{...field}
								placeholder="np. TEAM-01"
								style={{ textTransform: "uppercase" }}
								onChange={(_, data) => field.onChange(data.value.toUpperCase())}
							/>
						)}
					/>
				</Field>

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
							maxLength: {
								value: 100,
								message: "Maksymalnie 100 znaków",
							},
						}}
						render={({ field }) => (
							<Input {...field} placeholder="Wprowadź nazwę zespołu" />
						)}
					/>
				</Field>
			</div>

			<Field
				label="Struktura *"
				validationState={errors.structureId ? "error" : "none"}
				validationMessage={errors.structureId?.message}
			>
				<Controller
					name="structureId"
					control={control}
					rules={{
						required: "Struktura jest wymagana",
						validate: (value) => value > 0 || "Wybierz Strukturę",
					}}
					render={() => (
						<Dropdown
							value={selectedSbu?.name || ""}
							selectedOptions={
								selectedStructureId ? [String(selectedStructureId)] : []
							}
							onOptionSelect={handleSbuChange}
							placeholder="Wybierz Strukturę"
						>
							{sbuOptions.map((sbu) => (
								<Option
									key={sbu.id}
									value={String(sbu.id)}
									text={`${sbu.code} - ${sbu.name}`}
								>
									{sbu.code} - {sbu.name}
								</Option>
							))}
						</Dropdown>
					)}
				/>
			</Field>

			<Field
				label="Opis"
				validationState={errors.description ? "error" : "none"}
				validationMessage={errors.description?.message}
			>
				<Controller
					name="description"
					control={control}
					rules={{
						maxLength: {
							value: 500,
							message: "Maksymalnie 500 znaków",
						},
					}}
					render={({ field }) => (
						<Textarea {...field} placeholder="Opis zespołu..." rows={3} />
					)}
				/>
			</Field>

			{isEditMode && (
				<Field label=" ">
					<Controller
						name="isActive"
						control={control}
						render={({ field }) => (
							<Checkbox
								checked={field.value}
								onChange={(_, data) => field.onChange(data.checked)}
								label="Zespół aktywny"
							/>
						)}
					/>
				</Field>
			)}

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

export default TeamForm;
