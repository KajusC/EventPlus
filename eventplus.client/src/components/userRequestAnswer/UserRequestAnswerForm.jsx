import React, { useState } from 'react';
import { 
    TextField, 
    Button, 
    Box, 
    Card, 
    CardContent, 
    Typography, 
    FormControl, 
    FormHelperText,
    CircularProgress
} from '@mui/material';
import { Send as SendIcon } from '@mui/icons-material';
import { useAuth } from '../../context/AuthContext';
import { createUserRequestAnswer } from '../../services/userRequestAnswerService';
import { useNotification } from '../../context/NotificationContext';

const UserRequestAnswerForm = ({ questionId, onRequestSubmitted }) => {
    const [answer, setAnswer] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { currentUser } = useAuth();
    const { showSuccess, showError } = useNotification();

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (!answer.trim()) {
            showError('Atsakymas negali būti tuščias');
            return;
        }
        
        setIsSubmitting(true);
        
        try {
            const requestData = {
                IdUserRequestAnswer: 0,
                Answer: answer,
                FkQuestionidQuestion: questionId,
                FkUseridUser: currentUser?.id,

        };
            
            console.log('Siunčiami duomenys:', requestData);
            
            await createUserRequestAnswer(requestData);
            showSuccess('Jūsų užklausa sėkmingai išsiųsta!');
            setAnswer('');
            
            if (onRequestSubmitted) {
                onRequestSubmitted();
            }
        } catch (error) {
            showError(`Klaida siunčiant užklausą: ${error.message}`);
        } finally {
            setIsSubmitting(false);
        }
    };
    
    return (
        <Card sx={{ mb: 3, borderRadius: 2, boxShadow: '0 4px 12px rgba(0,0,0,0.08)' }}>
            <CardContent sx={{ p: 3 }}>
                <Typography variant="h6" component="h2" sx={{ mb: 2, fontWeight: 600 }}>
                    Pateikite savo užklausą
                </Typography>
                <Box component="form" onSubmit={handleSubmit}>
                    <FormControl fullWidth sx={{ mb: 2 }}>
                        <TextField
                            label="Jūsų atsakymas"
                            multiline
                            rows={4}
                            value={answer}
                            onChange={(e) => setAnswer(e.target.value)}
                            variant="outlined"
                            required
                            disabled={isSubmitting}
                        />
                        <FormHelperText>
                            Aprašykite savo užklausą kuo detaliau
                        </FormHelperText>
                    </FormControl>
                    <Button 
                        type="submit" 
                        variant="contained" 
                        color="primary"
                        disabled={isSubmitting} 
                        endIcon={isSubmitting ? <CircularProgress size={16} /> : <SendIcon />}
                        sx={{ 
                            py: 1,
                            px: 3,
                            background: 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)',
                            fontWeight: 600
                        }}
                    >
                        {isSubmitting ? 'Siunčiama...' : 'Siųsti užklausą'}
                    </Button>
                </Box>
            </CardContent>
        </Card>
    );
};

export default UserRequestAnswerForm;