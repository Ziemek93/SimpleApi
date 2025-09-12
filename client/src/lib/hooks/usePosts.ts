import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import agent from "../agent.ts";

export const usePosts = (id?: string) => {
    
    const queryClient = useQueryClient();
    
    const {data: posts, isPending: isPendingPosts} = useQuery({
        queryKey: ['posts'],
        queryFn: async () => {
            const response = await agent.get<PostListItem[]>('/articles')
            return response.data
        }
        
    });

    const {data: post, isPending: isPendingPost} = useQuery({
        queryKey: ['posts ', id],
        queryFn: async () => {
            const response = await agent.get<PostListItem>(`/article/${id}`);
            return response.data;
        },

        enabled: !!id
    });

    const updatePost = useMutation({
        mutationFn: async (post: PostListItem) => {
            agent.put(`/article/${post.id}`, post)
        },
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['posts']
            })
        }
    });

    const deletePost = useMutation({
        mutationFn: async (id: string) => {
            agent.delete(`/article/${id}`)
        },
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['posts']
            })
        }
    });

    const createPost = useMutation({
        mutationFn: async (post: PostListItem) => {
            agent.post(`/article/create`, post)
        },
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['posts']
            })
        }
    });

    return {
        posts,
        post,
        isPendingPosts,
        isPendingPost,
        updatePost,
        deletePost,
        createPost
    }

    // {
    //     "articleId": 0,
    //     "articleName": "string",
    //     "articleDescription": "string",
    //     "visibility": true,
    //     "tags": [
    //     "string"
    // ]
    // }
    
}