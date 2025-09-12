import Typography from '@mui/material/Typography';
import React from 'react';


interface Props  {
    Title: string;
    Description: string;
    Visibility: boolean;
    Category: Category | null;
    Tags: string[];
}
export default function PostDetails({Title, Description, Category, Tags}: Props) {



    return (
        <div>
            <Typography variant="subtitle2" gutterBottom>
                {Tags.map((tag) => (
                    <React.Fragment>
                        {tag} + ' '
                    </React.Fragment>
                ))}
            </Typography>            
            <Typography variant="subtitle1" gutterBottom>
            {Category?.name}
        </Typography>
            <h2>{Title}</h2>
            <p>{Description}</p>
        </div>
    );
}