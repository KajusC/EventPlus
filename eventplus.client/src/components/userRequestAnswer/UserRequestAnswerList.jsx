import React, { useState, useEffect } from 'react';
import { 
    Box, 
    Typography, 
    CircularProgress, 
    Alert, 
    Tabs, 
    Tab, 
    Paper 
} from '@mui/material';
import UserRequestAnswerItem from './UserRequestAnswerItem';
import { fetchUserRequestAnswers, fetchUserRequestAnswersByUserId } from '../../services/userRequestAnswerService';
import { useAuth } from '../../context/AuthContext';

const UserRequestAnswerList = () => {
    const [UserRequestAnswers, setUserRequestAnswers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [tabValue, setTabValue] = useState(0);
    
    const { currentUser, isAdmin, isOrganizer } = useAuth();
    
    const loadUserRequestAnswers = async () => {
        setLoading(true);
        setError(null);
        
        try {
            let requests;
            
            if (isAdmin || isOrganizer) {
                // Administratoriai ir organizatoriai mato visas užklausas
                requests = await fetchUserRequestAnswers();
            } else {
                // Paprasti vartotojai mato tik savo užklausas
                requests = await fetchUserRequestAnswersByUserId(currentUser.id);
            }
            
            setUserRequestAnswers(requests);
        } catch (error) {
            console.error('Error loading user requests:', error);
            setError('Įvyko klaida kraunant užklausas. Bandykite dar kartą vėliau.');
        } finally {
            setLoading(false);
        }
    };
    
    useEffect(() => {
        loadUserRequestAnswers();
    }, [currentUser?.id, isAdmin, isOrganizer]);
    
    const handleTabChange = (event, newValue) => {
        setTabValue(newValue);
    };
    
    const handleRequestUpdate = (updatedRequest) => {
        setUserRequestAnswers(prevRequests => 
            prevRequests.map(req => 
                req.idUserRequestAnswerAnswer === updatedRequest.idUserRequestAnswerAnswer ? updatedRequest : req
            )
        );
    };
    
    const handleRequestDelete = (deletedId) => {
        setUserRequestAnswers(prevRequests => 
            prevRequests.filter(req => req.idUserRequestAnswerAnswer !== deletedId)
        );
    };
    
    const filteredRequests = tabValue === 0 
        ? UserRequestAnswers 
        : tabValue === 1 
            ? UserRequestAnswers.filter(req => req.isProcessed) 
            : UserRequestAnswers.filter(req => !req.isProcessed);
    
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
            <Paper sx={{ mb: 1 }}>
                <Tabs 
                    value={tabValue} 
                    onChange={handleTabChange}
                    variant="fullWidth"
                    sx={{ borderBottom: 1, borderColor: 'divider' }}
                >
                    <Tab label="Visos užklausos" />
                </Tabs>
            </Paper>
            
            {filteredRequests.length > 0 ? (
                filteredRequests.map(request => (
                    <UserRequestAnswerItem 
                        key={request.idUserRequestAnswerAnswer} 
                        UserRequestAnswer={request} 
                        onUpdate={handleRequestUpdate}
                        onDelete={handleRequestDelete}
                    />
                ))
            ) : (
                <Typography variant="body1" color="text.secondary" align="center" sx={{ mt: 3 }}>
                    Nėra {tabValue === 0 ? "jokių" : tabValue === 1 ? "atsakytų" : "laukiančių"} užklausų.
                </Typography>
            )}
        </Box>
    );
};

export default UserRequestAnswerList;