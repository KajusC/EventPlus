import React, { useState, useEffect } from 'react';
import { 
    Box, 
    Tabs,
    Tab,
    Paper,
    Typography,
    CircularProgress,
    Alert
} from '@mui/material';
import { useAuth } from '../../context/AuthContext';
import { fetchUserRequestAnswers, fetchUserRequestAnswersByUserId } from '../../services/userRequestAnswerService';
import UserRequestAnswerItem from './UserRequestAnswerItem';

const UserRequestAnswerList = () => {
    const [userRequestAnswers, setUserRequestAnswers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [tabValue, setTabValue] = useState(0);
    
    const { currentUser, isAdmin, isOrganizer } = useAuth();
    
    const loadUserRequestAnswers = async () => {
        try {
            setLoading(true);
            let requests;
            
            if (isAdmin || isOrganizer) {
                // Administrators and organizers can see all requests
                requests = await fetchUserRequestAnswers();
            } else {
                // Regular users can only see their own requests
                requests = await fetchUserRequestAnswersByUserId(currentUser.id);
            }
            
            setUserRequestAnswers(requests);
            setError(null);
        } catch (error) {
            console.error('Error loading user requests:', error);
            setError('Įvyko klaida kraunant atsakymus. Bandykite dar kartą vėliau.');
        } finally {
            setLoading(false);
        }
    };
    
    useEffect(() => {
        if (currentUser?.id) {
            loadUserRequestAnswers();
        }
    }, [currentUser?.id, isAdmin, isOrganizer]);
    
    const handleTabChange = (event, newValue) => {
        setTabValue(newValue);
    };
    
    const handleRequestUpdate = (updatedRequest) => {
        setUserRequestAnswers(prevRequests => 
            prevRequests.map(req => 
                req.idUserRequestAnswer === updatedRequest.idUserRequestAnswer ? updatedRequest : req
            )
        );
    };
    
    const handleRequestDelete = (deletedId) => {
        setUserRequestAnswers(prevRequests => 
            prevRequests.filter(req => req.idUserRequestAnswer !== deletedId)
        );
    };
    
    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
                <CircularProgress />
            </Box>
        );
    }
    
    if (error) {
        return <Alert severity="error">{error}</Alert>;
    }
    
    return (
        <Box>
            <Paper sx={{ mb: 2 }}>
                <Tabs 
                    value={tabValue} 
                    onChange={handleTabChange}
                    variant="fullWidth"
                    sx={{ borderBottom: 1, borderColor: 'divider' }}
                >
                    <Tab label="Visi atsakymai" />
                    {(isAdmin || isOrganizer) && <Tab label="Apdoroti" />}
                    {(isAdmin || isOrganizer) && <Tab label="Neapdoroti" />}
                </Tabs>
            </Paper>
            
            {userRequestAnswers.length > 0 ? (
                userRequestAnswers.map(request => (
                    <UserRequestAnswerItem 
                        key={request.idUserRequestAnswer} 
                        UserRequestAnswer={request} 
                        onUpdate={handleRequestUpdate}
                        onDelete={handleRequestDelete}
                    />
                ))
            ) : (
                <Typography variant="body1" color="text.secondary" align="center" sx={{ mt: 3 }}>
                    Nėra atsakymų, kuriuos būtų galima parodyti.
                </Typography>
            )}
        </Box>
    );
};

export default UserRequestAnswerList;