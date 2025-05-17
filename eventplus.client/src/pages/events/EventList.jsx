import React, { useState, useEffect } from "react";
import {
	Grid,
	Container,
	Typography,
	Box,
	Button,
	Paper,
	InputBase,
	IconButton,
	Divider,
	Chip,
} from "@mui/material";

import {
	Search as SearchIcon,
	Add as AddIcon,
	FilterList,
} from "@mui/icons-material";
import { Link } from "react-router-dom";

import EventCard from "../../components/events/EventCard";
import { fetchEvents } from "../../services/eventService";

import LoadingSpinner from "../../components/shared/LoadingSpinner";
import ErrorDisplay from "../../components/shared/ErrorDisplay";
import { useAuth } from "../../context/AuthContext";

function EventList() {
	const [events, setEvents] = useState([]);
	const [isLoading, setIsLoading] = useState(true);
	const [error, setError] = useState(null);
	const [searchTerm, setSearchTerm] = useState("");
	const { isAdmin, isOrganizer } = useAuth();
	useEffect(() => {
		const getEvents = async () => {
			try {
				setIsLoading(true);
				const data = await fetchEvents();
				setEvents(data);
				setError(null);
			} catch (error) {
				console.error("Error fetching events:", error);
				setError("Failed to load events. Please try again later.");
			} finally {
				setIsLoading(false);
			}
		};

		getEvents();
	}, []);

	const filteredEvents = searchTerm
		? events.filter((event) =>
				event.name.toLowerCase().includes(searchTerm.toLowerCase())
		  )
		: events;

	return (
		<>
			<Box sx={{ width: "100%", minHeight: "100vh", pb: 8 }}>
				{/* Hero Section */}
				<Box
					sx={{
						background: "linear-gradient(135deg, #1a1a2e 0%, #16213e 100%)",
						pt: { xs: 12, md: 16 },
						pb: { xs: 8, md: 10 },
						px: 2,
						position: "relative",
						overflow: "hidden",
						"&::before": {
							content: '""',
							position: "absolute",
							top: 0,
							left: 0,
							right: 0,
							bottom: 0,
							background:
								"url(https://source.unsplash.com/random/1600x800/?concert) center/cover no-repeat",
							opacity: 0.2,
							zIndex: 0,
						},
						"&::after": {
							content: '""',
							position: "absolute",
							top: 0,
							left: 0,
							right: 0,
							bottom: 0,
							background:
								"radial-gradient(circle at center, rgba(106,17,203,0.3) 0%, rgba(37,117,252,0) 70%)",
							zIndex: 1,
						},
					}}
				>
					<Container maxWidth="lg" sx={{ position: "relative", zIndex: 2 }}>
						<Box sx={{ textAlign: "center", mb: 5 }}>
							<Typography
								variant="h2"
								component="h1"
								color="white"
								sx={{
									fontWeight: 800,
									mb: 2,
									textShadow: "0 2px 10px rgba(0,0,0,0.3)",
									fontSize: { xs: "2.5rem", md: "3.5rem" },
								}}
							>
								Discover Amazing Events
							</Typography>
							<Typography
								variant="h6"
								color="white"
								sx={{
									opacity: 0.8,
									maxWidth: "700px",
									mx: "auto",
									mb: 4,
									fontWeight: 400,
								}}
							>
								Find and book tickets for the best events happening near you
							</Typography>
							<Paper
								component="form"
								sx={{
									p: "2px 4px",
									display: "flex",
									alignItems: "center",
									width: { xs: "100%", sm: "80%", md: "600px" },
									mx: "auto",
									borderRadius: 3,
									boxShadow: "0 4px 20px rgba(0,0,0,0.1)",
								}}
							>
								<IconButton sx={{ p: "10px" }} aria-label="search">
									<SearchIcon />
								</IconButton>
								<InputBase
									sx={{ ml: 1, flex: 1 }}
									placeholder="Search events..."
									inputProps={{ "aria-label": "search events" }}
									value={searchTerm}
									onChange={(e) => setSearchTerm(e.target.value)}
								/>
								<Divider sx={{ height: 28, m: 0.5 }} orientation="vertical" />
								<IconButton sx={{ p: "10px" }} aria-label="filters">
									<FilterList />
								</IconButton>
							</Paper>
						</Box>

						<Box
							sx={{
								display: "flex",
								justifyContent: "center",
								gap: 1,
								flexWrap: "wrap",
							}}
						>
							<Chip
								label="All Events"
								color="primary"
								sx={{
									background:
										"linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)",
									color: "white",
									fontWeight: 600,
									px: 1,
								}}
							/>
							<Chip
								label="Music"
								variant="outlined"
								sx={{ color: "white", borderColor: "rgba(255,255,255,0.5)" }}
							/>
							<Chip
								label="Business"
								variant="outlined"
								sx={{ color: "white", borderColor: "rgba(255,255,255,0.5)" }}
							/>
							<Chip
								label="Art & Culture"
								variant="outlined"
								sx={{ color: "white", borderColor: "rgba(255,255,255,0.5)" }}
							/>
							<Chip
								label="Sports"
								variant="outlined"
								sx={{ color: "white", borderColor: "rgba(255,255,255,0.5)" }}
							/>
							<Chip
								label="Technology"
								variant="outlined"
								sx={{ color: "white", borderColor: "rgba(255,255,255,0.5)" }}
							/>
						</Box>
					</Container>
				</Box>

				{/* Events List */}
				<Container
					maxWidth="lg"
					sx={{ mt: -5, position: "relative", zIndex: 3 }}
				>
					{(isAdmin() || isOrganizer()) && (
											<Box
											sx={{
												display: "flex",
												justifyContent: "space-between",
												alignItems: "center",
												mb: 4,
												backgroundColor: "white",
												borderRadius: 3,
												p: 3,
												boxShadow: "0 4px 20px rgba(0,0,0,0.1)",
											}}
										>
											<Typography
												variant="h5"
												component="h2"
												fontWeight={700}
												color="text.primary"
											>
												{filteredEvents.length === 0 && !isLoading
													? "No events found"
													: "Upcoming Events"}
											</Typography>
											<Button
												component={Link}
												to="/eventinsert"
												variant="contained"
												startIcon={<AddIcon />}
												sx={{
													background: "linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)",
													boxShadow: "0 3px 5px 2px rgba(106, 17, 203, .3)",
													color: "white",
													fontWeight: 600,
													borderRadius: 2,
													px: 3,
													"&:hover": {
														boxShadow: "0 5px 8px 2px rgba(106, 17, 203, .35)",
													},
												}}
											>
												Create Event
											</Button>
										</Box>
						)}


					{/* Use shared components for loading and error states */}
					{isLoading ? (
						<LoadingSpinner fullHeight={false} />
					) : error ? (
						<ErrorDisplay error={error} marginTop={2} marginBottom={4} />
					) : (
						<Grid container spacing={3}>
							{filteredEvents.map((event) => (
								<Grid key={event.idEvent} item xs={12} sm={6} md={4} lg={4}>
									<EventCard event={event} />
								</Grid>
							))}
						</Grid>
					)}

					{/* No events found message */}
					{!isLoading && filteredEvents.length === 0 && (
						<Box textAlign="center" py={8}>
							<Typography variant="h6" color="text.secondary" mb={2}>
								No events found matching your search.
							</Typography>
							<Button
								variant="outlined"
								onClick={() => setSearchTerm("")}
								sx={{
									borderColor: "#6a11cb",
									color: "#6a11cb",
									"&:hover": {
										borderColor: "#2575fc",
										backgroundColor: "rgba(106, 17, 203, 0.04)",
									},
								}}
							>
								Clear Search
							</Button>
						</Box>
					)}
				</Container>
			</Box>
		</>
	);
}

export default EventList;
