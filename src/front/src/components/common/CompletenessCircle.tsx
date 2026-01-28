import { makeStyles, tokens, Tooltip } from "@fluentui/react-components";
import type React from "react";

const SIZE = 36;
const STROKE_WIDTH = 4;

type ColorTheme = "error" | "warning" | "success";

function getColor(percent: number): ColorTheme {
	if (percent < 30) return "error";
	if (percent < 60) return "warning";
	return "success";
}

const useStyles = makeStyles({
	container: {
		position: "relative",
		display: "inline-flex",
		alignItems: "center",
		justifyContent: "center",
	},
	svg: {
		transform: "rotate(-90deg)",
	},
	trackError: {
		stroke: tokens.colorPaletteRedBackground2,
	},
	trackWarning: {
		stroke: tokens.colorPaletteYellowBackground2,
	},
	trackSuccess: {
		stroke: tokens.colorPaletteGreenBackground2,
	},
	progressError: {
		stroke: tokens.colorPaletteRedForeground1,
		transition: "stroke-dashoffset 0.3s ease",
	},
	progressWarning: {
		stroke: tokens.colorPaletteYellowForeground1,
		transition: "stroke-dashoffset 0.3s ease",
	},
	progressSuccess: {
		stroke: tokens.colorPaletteGreenForeground1,
		transition: "stroke-dashoffset 0.3s ease",
	},
	label: {
		position: "absolute",
		fontSize: tokens.fontSizeBase100,
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorNeutralForeground1,
	},
});

export interface CompletenessCircleProps {
	value: number;
	showLabel?: boolean;
	size?: number;
}

const CompletenessCircle: React.FC<CompletenessCircleProps> = ({
	value,
	showLabel = true,
	size = SIZE,
}) => {
	const styles = useStyles();
	const percent = Math.max(0, Math.min(100, value));
	const colorTheme = getColor(percent);

	const radius = (size - STROKE_WIDTH) / 2;
	const circumference = 2 * Math.PI * radius;
	const strokeDashoffset = circumference - (percent / 100) * circumference;

	const trackClass =
		colorTheme === "error"
			? styles.trackError
			: colorTheme === "warning"
				? styles.trackWarning
				: styles.trackSuccess;

	const progressClass =
		colorTheme === "error"
			? styles.progressError
			: colorTheme === "warning"
				? styles.progressWarning
				: styles.progressSuccess;

	const scaleFactor = size / SIZE;
	const fontSize = 10 * scaleFactor;

	return (
		<Tooltip content={`Kompletność: ${percent}%`} relationship="description">
			<div className={styles.container} style={{ width: size, height: size }}>
				<svg
					className={styles.svg}
					width={size}
					height={size}
					viewBox={`0 0 ${size} ${size}`}
				>
					<circle
						className={trackClass}
						cx={size / 2}
						cy={size / 2}
						r={radius}
						fill="none"
						strokeWidth={STROKE_WIDTH}
					/>
					<circle
						className={progressClass}
						cx={size / 2}
						cy={size / 2}
						r={radius}
						fill="none"
						strokeWidth={STROKE_WIDTH}
						strokeLinecap="round"
						strokeDasharray={circumference}
						strokeDashoffset={strokeDashoffset}
					/>
				</svg>
				{showLabel && (
					<span className={styles.label} style={{ fontSize: `${fontSize}px` }}>
						{percent}
					</span>
				)}
			</div>
		</Tooltip>
	);
};

export default CompletenessCircle;
