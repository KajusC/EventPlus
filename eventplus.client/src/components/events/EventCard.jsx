import React from 'react';
import { Card, CardContent, Typography, Button, CardActions, Box, Avatar, useTheme } from '@mui/material';
import { styled } from '@mui/system';
import { NavLink } from 'react-router-dom';

const StyledCard = styled(Card)(({ theme }) => ({
    height: '100%',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'space-between',
    borderRadius: theme.spacing(2),
    boxShadow: '0px 7px 20px rgba(0, 0, 0, 0.08)',
    transition: 'transform 0.3s ease-in-out',
    '&:hover': {
        transform: 'scale(1.05)',
    },
}));

const StyledCardContent = styled(CardContent)(({ theme }) => ({
    padding: theme.spacing(3),
}));

const StyledCardActions = styled(CardActions)(({ theme }) => ({
    padding: theme.spacing(0, 3, 3, 3),
    display: 'flex',
    justifyContent: 'space-between',
}));

const StyledButton = styled(Button)(({ theme }) => ({
    backgroundColor: (theme) => theme.palette.primary.main,
    color: (theme) => theme.palette.common.white,
    borderRadius: theme.spacing(1.5),
    padding: theme.spacing(1, 2),
    '&:hover': {
        backgroundColor: (theme) => theme.palette.primary.dark,
    },
}));

const formatDate = (dateString) => {
    const options = { year: 'numeric', month: 'long', day: 'numeric'};
    const date = new Date(dateString);
    options.hour = 'numeric';
    options.minute = 'numeric';
    return date.toLocaleString(undefined, options);
};

function EventCard({ event }) {
    const theme = useTheme();

    return (
      <StyledCard>
        <Box sx={{ display: "flex", flexDirection: "column", height: "100%" }}>
          <StyledCardContent>
            <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
              <Avatar
                sx={{
                  bgcolor: theme.palette.primary.main,
                  color: "white",
                  mr: 2,
                }}
              >
                {event.category}
              </Avatar>
              <Typography variant="h6" component="div">
                {event.name}
              </Typography>
            </Box>
            <Typography variant="body2" color="text.secondary">
              {event.description}
            </Typography>
            <Box mt={2}>
              <Typography variant="subtitle2">
                Category: {event.category}
              </Typography>
              <Typography variant="subtitle2">
                Start Date: {formatDate(event.startDate)}
              </Typography>
              <Typography variant="subtitle2">
                End Date: {formatDate(event.endDate)}
              </Typography>
              <Typography variant="subtitle2">
                Tickets Available: {event.maxTicketCount}
              </Typography>
            </Box>
          </StyledCardContent>
          <StyledCardActions sx={{ mt: "auto" }}>
            <StyledButton
              size="small"
              component={NavLink}
              to={`/eventview/${event.idEvent}`}
            >
              View Details
            </StyledButton>
          </StyledCardActions>
        </Box>
      </StyledCard>
    );
}

export default EventCard;