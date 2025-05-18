import React, { useState } from 'react';
import {
    Card, 
    CardContent, 
    Typography, 
    Box, 
    IconButton, 
    Collapse, 
    Divider,
    TextField,
    Button
} from '@mui/material';
import { 
    Edit as EditIcon, 
    Delete as DeleteIcon,
    ExpandMore as ExpandMoreIcon,
    ExpandLess as ExpandLessIcon,
    QuestionAnswer as QuestionIcon
} from '@mui/icons-material';
import { useAuth } from '../../context/AuthContext';
import { useNotification } from '../../context/NotificationContext';
import { updateUserRequestAnswer, deleteUserRequestAnswer } from '../../services/userRequestAnswerService';

const UserRequestAnswerItem = ({ UserRequestAnswer, onUpdate, onDelete }) => {
    const [expanded, setExpanded] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [editedAnswer, setEditedAnswer] = useState(UserRequestAnswer.answer);
    const [isSubmitting, setIsSubmitting] = useState(false);
    
    const { currentUser, isAdmin, isOrganizer } = useAuth();
    const { showSuccess, showError } = useNotification();
    
    const isOwner = currentUser?.id === UserRequestAnswer.fkUseridUser;
    const canEdit = isOwner || isAdmin || isOrganizer;
    const canDelete = isAdmin;
    
    const handleExpandClick = () => {
        setExpanded(!expanded);
    };
    
    const handleEditClick = () => {
        setIsEditing(true);
        setExpanded(true);
    };
    
    const handleCancelEdit = () => {
        setIsEditing(false);
        setEditedAnswer(UserRequestAnswer.answer);
    };
    
    const handleSaveEdit = async () => {
        setIsSubmitting(true);
        
        try {
            const updatedRequest = {
                ...UserRequestAnswer,
                answer: editedAnswer
            };
            
            await updateUserRequestAnswer(updatedRequest);
            showSuccess('Atsakymas sėkmingai atnaujintas!');
            setIsEditing(false);
            
            if (onUpdate) {
                onUpdate(updatedRequest);
            }
        } catch (error) {
            showError(`Klaida atnaujinant atsakymą: ${error.message}`);
        } finally {
            setIsSubmitting(false);
        }
    };
    
    const handleDelete = async () => {
        if (!window.confirm('Ar tikrai norite ištrinti šį atsakymą?')) {
            return;
        }
        
        setIsSubmitting(true);
        
        try {
            await deleteUserRequestAnswer(UserRequestAnswer.idUserRequestAnswer);
            showSuccess('Atsakymas sėkmingai ištrintas!');
            
            if (onDelete) {
                onDelete(UserRequestAnswer.idUserRequestAnswer);
            }
        } catch (error) {
            showError(`Klaida trinant atsakymą: ${error.message}`);
        } finally {
            setIsSubmitting(false);
        }
    };
    
    return (
        <Card sx={{ mb: 2, borderRadius: 2, boxShadow: '0 2px 8px rgba(0,0,0,0.05)' }}>
            <CardContent sx={{ pb: 1 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                        <QuestionIcon sx={{ mr: 1, color: 'primary.main' }} />
                        <Typography variant="h6" component="h3" sx={{ fontWeight: 600 }}>
                            {UserRequestAnswer.questionText || `Klausimas #${UserRequestAnswer.fkQuestionidQuestion}`}
                        </Typography>
                    </Box>
                    <Box>
                        <IconButton size="small" onClick={handleExpandClick}>
                            {expanded ? <ExpandLessIcon /> : <ExpandMoreIcon />}
                        </IconButton>
                    </Box>
                </Box>
                
                <Collapse in={expanded} timeout="auto" unmountOnExit>
                    <Box sx={{ mt: 2 }}>
                        <Divider sx={{ my: 1 }} />
                        
                        {isEditing ? (
                            <Box sx={{ mt: 2 }}>
                                <TextField
                                    multiline
                                    rows={3}
                                    fullWidth
                                    variant="outlined"
                                    value={editedAnswer}
                                    onChange={(e) => setEditedAnswer(e.target.value)}
                                    sx={{ mb: 2 }}
                                />
                                <Box sx={{ display: 'flex', gap: 1 }}>
                                    <Button 
                                        variant="contained" 
                                        onClick={handleSaveEdit}
                                        disabled={isSubmitting}
                                    >
                                        {isSubmitting ? "Išsaugoma..." : "Išsaugoti"}
                                    </Button>
                                    <Button 
                                        variant="outlined" 
                                        onClick={handleCancelEdit}
                                        disabled={isSubmitting}
                                    >
                                        Atšaukti
                                    </Button>
                                </Box>
                            </Box>
                        ) : (
                            <Typography variant="body1" paragraph>
                                {UserRequestAnswer.answer}
                            </Typography>
                        )}
                        
                        {!isEditing && (
                            <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 1 }}>
                                {canEdit && (
                                    <IconButton 
                                        size="small" 
                                        onClick={handleEditClick}
                                        disabled={isSubmitting}
                                    >
                                        <EditIcon fontSize="small" />
                                    </IconButton>
                                )}
                                {canDelete && (
                                    <IconButton 
                                        size="small" 
                                        color="error" 
                                        onClick={handleDelete}
                                        disabled={isSubmitting}
                                    >
                                        <DeleteIcon fontSize="small" />
                                    </IconButton>
                                )}
                            </Box>
                        )}
                    </Box>
                </Collapse>
            </CardContent>
        </Card>
    );
};

export default UserRequestAnswerItem;