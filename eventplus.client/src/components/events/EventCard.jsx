import {React, useEffect, useState} from "react";
import { useNavigate } from "react-router-dom";
import {
	Card,
	CardContent,
	Typography,
	Button,
	CardActions,
	Box,
	Chip,
	useTheme,
	CardMedia,
} from "@mui/material";
import { styled } from "@mui/system";
import { NavLink, Link } from "react-router-dom";
import {
	CalendarMonth,
	ConfirmationNumber,
    Edit as EditIcon,
    DeleteOutline as DeleteIcon,
} from "@mui/icons-material";

import { fetchCategoryById } from "../../services/categoryService";
import { deleteEvent } from "../../services/eventService";

// Import shared components
import ToastNotification from "../shared/ToastNotification";
import ConfirmationDialog from "../shared/ConfirmationDialog";

const StyledCard = styled(Card)(({ theme }) => ({
	height: "100%",
	display: "flex",
	flexDirection: "column",
	justifyContent: "space-between",
	borderRadius: theme.spacing(2),
	boxShadow: "0px 10px 30px rgba(0, 0, 0, 0.1)",
	overflow: "hidden",
	transition: "all 0.3s ease-in-out",
	position: "relative",
	"&:hover": {
		transform: "translateY(-8px)",
		boxShadow: "0px 20px 40px rgba(0, 0, 0, 0.2)",
	},
}));

const CardImageOverlay = styled(Box)(({ theme }) => ({
	position: "absolute",
	top: 0,
	left: 0,
	right: 0,
	height: "35%",
	background: "linear-gradient(180deg, rgba(0,0,0,0.7) 0%, rgba(0,0,0,0) 100%)",
	zIndex: 1,
}));

const CardTopContent = styled(Box)(({ theme }) => ({
	position: "relative",
	zIndex: 2,
	padding: theme.spacing(2),
	display: "flex",
	justifyContent: "space-between",
	alignItems: "flex-start",
}));

const StyledCardContent = styled(CardContent)(({ theme }) => ({
	padding: theme.spacing(3),
	paddingTop: theme.spacing(1),
	flexGrow: 1,
}));

const StyledCardActions = styled(CardActions)(({ theme }) => ({
	padding: theme.spacing(2, 3, 3),
	display: "flex",
	justifyContent: "space-between",
	alignItems: "center",
	borderTop: "1px solid rgba(0,0,0,0.05)",
}));

const formatDate = (dateString) => {
	const options = { year: "numeric", month: "short", day: "numeric" };
	const date = new Date(dateString);
	return date.toLocaleString(undefined, options);
};

const formatTime = (dateString) => {
	const options = { hour: "numeric", minute: "numeric" };
	const date = new Date(dateString);
	return date.toLocaleString(undefined, options);
};

function EventCard({ event }) {
	const navigate = useNavigate();
	const theme = useTheme();
	
	// Simplified state 
	const [categoryName, setCategoryName] = useState("Loading...");
	const [isDeleting, setIsDeleting] = useState(false);
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);
	const [toast, setToast] = useState({
		open: false,
		message: "",
		severity: "info",
	});

	// Fetch category name
	useEffect(() => {
		const getCategoryName = async () => {
		  try {
			const category = await fetchCategoryById(event.category);
			setCategoryName(category.name || "Unknown Category");
		  } catch (error) {
			setCategoryName("Unknown Category");
		  }
		};
		getCategoryName();
	}, [event.category]);
				
	// Toast notification handler
	const handleCloseToast = () => {
		setToast({ ...toast, open: false });
	};

	// Delete handlers
	const handleDeleteConfirm = async () => {
		setIsDeleting(true);
		try {
			await deleteEvent(event.idEvent);
			setToast({
				open: true,
				message: "Event deleted successfully",
				severity: "success"
			});
			// Reload the page after a delay to show updated list
			setTimeout(() => window.location.reload(), 1500);
		} catch (err) {
			setToast({
				open: true,
				message: `Error: ${err.message || 'Failed to delete event'}`,
				severity: "error"
			});
		} finally {
			setIsDeleting(false);
			setShowDeleteDialog(false);
		}
	};

	return (
		<>
			<StyledCard>
				<Box sx={{ position: "relative" }}>
					<CardImageOverlay />
					<CardTopContent>
						<Chip
							label={categoryName}
							color="secondary"
							size="small"
							sx={{
								fontWeight: 600,
								backgroundColor: "rgba(106, 17, 203, 0.8)",
								color: "white",
								backdropFilter: "blur(4px)",
								"& .MuiChip-label": {
									px: 1.5,
								},
							}}
						/>
					</CardTopContent>
				</Box>

				<StyledCardContent>
					<Typography
						variant="h6"
						component="h2"
						gutterBottom
						sx={{
							fontWeight: 700,
							fontSize: "1.1rem",
							overflow: "hidden",
							textOverflow: "ellipsis",
							display: "-webkit-box",
							WebkitLineClamp: 2,
							WebkitBoxOrient: "vertical",
							height: "2.8rem",
							mb: 1.5,
						}}
					>
						{event.name}
					</Typography>

					<Typography
						variant="body2"
						color="text.secondary"
						sx={{
							overflow: "hidden",
							textOverflow: "ellipsis",
							display: "-webkit-box",
							WebkitLineClamp: 3,
							WebkitBoxOrient: "vertical",
							height: "4.5rem",
							mb: 2,
						}}
					>
						{event.description}
					</Typography>

					<Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
						<CalendarMonth
							sx={{ color: "text.secondary", fontSize: 20, mr: 1 }}
						/>
						<Typography variant="body2" color="text.primary" fontWeight={500}>
							{formatDate(event.startDate)} · {formatTime(event.startDate)}
						</Typography>
					</Box>

					<Box sx={{ display: "flex", alignItems: "center" }}>
						<ConfirmationNumber
							sx={{ color: "text.secondary", fontSize: 20, mr: 1 }}
						/>
						<Typography variant="body2" color="text.primary" fontWeight={500}>
							{event.maxTicketCount} tickets available
						</Typography>
					</Box>
				</StyledCardContent>

				<StyledCardActions sx={{ justifyContent: "flex-end" }}>
					<Button
						variant="contained"
						component={NavLink}
						to={`/eventview/${event.idEvent}`}
						size="small"
						sx={{
							fontWeight: 600,
							borderRadius: 2,
							px: 2.5,
							py: 0.8,
							textTransform: "none",
							background: "linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)",
							boxShadow: "0 3px 5px 2px rgba(106, 17, 203, .3)",
							"&:hover": {
								boxShadow: "0 5px 8px 2px rgba(106, 17, 203, .35)",
							},
						}}
					>
						View Details
					</Button>
				</StyledCardActions>
				<Box sx={{ display: "flex", justifyContent: "flex-end", gap: 1, p: 2 }}>
					<Button
						component={Link}
						to={`/eventedit/${event.idEvent}`}
						variant="outlined"
						color="primary"
						startIcon={<EditIcon />}
						sx={{
							borderColor: "#6a11cb",
							color: "#6a11cb",
							"&:hover": {
								borderColor: "#2575fc",
								bgcolor: "rgba(106, 17, 203, 0.04)",
							},
						}}
					>
						Edit
					</Button>
					<Button
						variant="outlined"
						color="error"
						startIcon={<DeleteIcon />}
						onClick={() => setShowDeleteDialog(true)}
					>
						Delete
					</Button>
				</Box>
			</StyledCard>

			{/* Using shared ConfirmationDialog component */}
			<ConfirmationDialog
				open={showDeleteDialog}
				title="Confirm Event Deletion"
				message="Are you sure you want to delete this event? This action cannot be undone."
				confirmLabel="Delete Event"
				cancelLabel="Cancel"
				onConfirm={handleDeleteConfirm}
				onCancel={() => setShowDeleteDialog(false)}
				loading={isDeleting}
				isDestructive={true}
			/>

			{/* Using shared ToastNotification component */}
			<ToastNotification
				open={toast.open}
				message={toast.message}
				severity={toast.severity}
				onClose={handleCloseToast}
			/>
		</>
	);
}

export default EventCard;
