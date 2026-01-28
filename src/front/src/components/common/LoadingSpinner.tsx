import type { SpinnerProps } from "@fluentui/react-components";
import { makeStyles, Spinner } from "@fluentui/react-components";
import type React from "react";

const useStyles = makeStyles({
	container: {
		display: "flex",
		alignItems: "center",
		justifyContent: "center",
		padding: "24px",
	},
	fullPage: {
		position: "fixed",
		inset: 0,
		backgroundColor: "rgba(255, 255, 255, 0.8)",
		zIndex: 1000,
	},
});

export interface LoadingSpinnerProps extends SpinnerProps {
	fullPage?: boolean;
}

const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({
	fullPage = false,
	label = "Ładowanie...",
	size = "medium",
	...props
}) => {
	const styles = useStyles();

	return (
		<div className={`${styles.container} ${fullPage ? styles.fullPage : ""}`}>
			<Spinner label={label} size={size} {...props} />
		</div>
	);
};

export default LoadingSpinner;
